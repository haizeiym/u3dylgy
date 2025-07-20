using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace YangLeGeYang2D.LevelEditor
{
    public class LevelValidator2D : MonoBehaviour
    {
        [System.Serializable]
        public class ValidationResult
        {
            public bool isValid;
            public List<string> errors = new List<string>();
            public List<string> warnings = new List<string>();
            public int totalCards;
            public int uniqueTypes;
            public Dictionary<int, int> typeCounts = new Dictionary<int, int>();
            public int totalIrregularShapes;
            public Dictionary<int, int> layerShapeCounts = new Dictionary<int, int>();
        }
        
        public static ValidationResult ValidateLevel(LevelData2D levelData)
        {
            ValidationResult result = new ValidationResult();
            result.totalCards = levelData.cards.Count;
            result.totalIrregularShapes = levelData.irregularShapes.Count;
            
            // 检查基本条件
            if (levelData.cards.Count == 0)
            {
                result.errors.Add("关卡没有卡片");
                result.isValid = false;
                return result;
            }
            
            // 统计卡片类型
            Dictionary<int, int> typeCounts = new Dictionary<int, int>();
            foreach (var card in levelData.cards)
            {
                if (!typeCounts.ContainsKey(card.type))
                {
                    typeCounts[card.type] = 0;
                }
                typeCounts[card.type]++;
            }
            
            result.typeCounts = typeCounts;
            result.uniqueTypes = typeCounts.Count;
            
            // 检查每种类型的卡片数量是否为3的倍数
            foreach (var kvp in typeCounts)
            {
                if (kvp.Value % 3 != 0)
                {
                    result.errors.Add($"卡片类型 {kvp.Key} 的数量 ({kvp.Value}) 不是3的倍数");
                    result.isValid = false;
                }
            }
            
            // 检查总卡片数是否为3的倍数
            if (levelData.cards.Count % 3 != 0)
            {
                result.errors.Add($"总卡片数 ({levelData.cards.Count}) 不是3的倍数");
                result.isValid = false;
            }
            
            // 检查层级设置
            if (levelData.totalLayers <= 0)
            {
                result.errors.Add("总层数必须大于0");
                result.isValid = false;
            }
            
            // 检查是否有卡片超出层级范围
            foreach (var card in levelData.cards)
            {
                if (card.layer < 0 || card.layer >= levelData.totalLayers)
                {
                    result.errors.Add($"卡片 {card.id} 的层级 ({card.layer}) 超出范围 [0, {levelData.totalLayers - 1}]");
                    result.isValid = false;
                }
            }
            
            // 检查位置重叠
            CheckPositionOverlaps2D(levelData.cards, result);
            
            // 检查不规则图形
            CheckIrregularShapes(levelData, result);
            
            // 检查卡片是否在不规则图形内
            CheckCardsInIrregularShapes(levelData, result);
            
            // 检查可解性（简化版本）
            if (result.errors.Count == 0)
            {
                CheckSolvability2D(levelData.cards, result);
            }
            
            // 如果没有错误，标记为有效
            if (result.errors.Count == 0)
            {
                result.isValid = true;
            }
            
            return result;
        }
        
        private static void CheckIrregularShapes(LevelData2D levelData, ValidationResult result)
        {
            // 统计每层的图形数量
            Dictionary<int, int> layerShapeCounts = new Dictionary<int, int>();
            foreach (var shape in levelData.irregularShapes)
            {
                if (!layerShapeCounts.ContainsKey(shape.layer))
                {
                    layerShapeCounts[shape.layer] = 0;
                }
                layerShapeCounts[shape.layer]++;
                
                // 检查图形是否有效
                if (shape.vertices.Count < 3)
                {
                    result.errors.Add($"不规则图形在层级 {shape.layer} 的顶点数量不足 (需要至少3个顶点)");
                    result.isValid = false;
                }
                
                // 检查图形是否超出边界
                Vector2 actualSize = levelData.useCustomAreaSize ? levelData.areaSize : new Vector2(
                    levelData.gridSize.x * levelData.cardSpacing,
                    levelData.gridSize.y * levelData.cardSpacing
                );
                
                foreach (var vertex in shape.vertices)
                {
                    if (Mathf.Abs(vertex.x) > actualSize.x * 0.5f || Mathf.Abs(vertex.y) > actualSize.y * 0.5f)
                    {
                        result.warnings.Add($"不规则图形在层级 {shape.layer} 的顶点 {vertex} 超出边界");
                    }
                }
            }
            
            result.layerShapeCounts = layerShapeCounts;
            
            // 检查是否有层级没有不规则图形
            for (int layer = 0; layer < levelData.totalLayers; layer++)
            {
                if (!layerShapeCounts.ContainsKey(layer) || layerShapeCounts[layer] == 0)
                {
                    result.warnings.Add($"层级 {layer} 没有不规则图形，卡片可以放置在任何位置");
                }
            }
        }
        
        private static void CheckCardsInIrregularShapes(LevelData2D levelData, ValidationResult result)
        {
            foreach (var card in levelData.cards)
            {
                bool isInAnyShape = false;
                
                foreach (var shape in levelData.irregularShapes)
                {
                    if (shape.layer == card.layer && shape.isActive)
                    {
                        if (IsPointInPolygon(card.position, shape.vertices))
                        {
                            isInAnyShape = true;
                            break;
                        }
                    }
                }
                
                if (!isInAnyShape)
                {
                    result.warnings.Add($"卡片 {card.id} 在层级 {card.layer} 的位置不在任何不规则图形内");
                }
            }
        }
        
        private static bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
        {
            if (polygon.Count < 3) return false;

            bool inside = false;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
                j = i;
            }

            return inside;
        }
        
        private static void CheckPositionOverlaps2D(List<CardData2D> cards, ValidationResult result)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = i + 1; j < cards.Count; j++)
                {
                    if (Vector2.Distance(cards[i].position, cards[j].position) < 0.1f)
                    {
                        result.errors.Add($"卡片 {cards[i].id} 和 {cards[j].id} 位置重叠");
                        result.isValid = false;
                    }
                }
            }
        }
        
        private static void CheckSolvability2D(List<CardData2D> cards, ValidationResult result)
        {
            // 简化的可解性检查
            // 检查是否有足够的顶层卡片可以被点击
            
            // 按层级分组
            Dictionary<int, List<CardData2D>> layerGroups = new Dictionary<int, List<CardData2D>>();
            foreach (var card in cards)
            {
                if (!layerGroups.ContainsKey(card.layer))
                {
                    layerGroups[card.layer] = new List<CardData2D>();
                }
                layerGroups[card.layer].Add(card);
            }
            
            // 检查顶层是否有可点击的卡片
            if (layerGroups.Count > 0)
            {
                int maxLayer = layerGroups.Keys.Max();
                if (layerGroups.ContainsKey(maxLayer))
                {
                    int topLayerCards = layerGroups[maxLayer].Count;
                    if (topLayerCards < 3)
                    {
                        result.warnings.Add($"顶层只有 {topLayerCards} 张卡片，可能无法开始游戏");
                    }
                }
            }
            
            // 检查是否有孤立卡片（被其他卡片完全阻挡）
            CheckBlockedCards2D(cards, result);
        }
        
        private static void CheckBlockedCards2D(List<CardData2D> cards, ValidationResult result)
        {
            // 简化的阻挡检查
            // 检查是否有卡片在相同位置但不同层级
            
            Dictionary<Vector2, List<CardData2D>> positionGroups = new Dictionary<Vector2, List<CardData2D>>();
            foreach (var card in cards)
            {
                if (!positionGroups.ContainsKey(card.position))
                {
                    positionGroups[card.position] = new List<CardData2D>();
                }
                positionGroups[card.position].Add(card);
            }
            
            foreach (var kvp in positionGroups)
            {
                if (kvp.Value.Count > 1)
                {
                    // 按层级排序
                    var sortedCards = kvp.Value.OrderBy(c => c.layer).ToList();
                    
                    // 检查是否有卡片被完全阻挡
                    for (int i = 0; i < sortedCards.Count - 1; i++)
                    {
                        if (sortedCards[i + 1].layer - sortedCards[i].layer <= 1)
                        {
                            result.warnings.Add($"卡片 {sortedCards[i + 1].id} 可能被卡片 {sortedCards[i].id} 阻挡");
                        }
                    }
                }
            }
        }
        
        public static string GetValidationReport(ValidationResult result)
        {
            string report = "=== 拔了个罐2D关卡验证报告 ===\n";
            
            if (result.isValid)
            {
                report += "✅ 2D关卡验证通过\n";
            }
            else
            {
                report += "❌ 2D关卡验证失败\n";
            }
            
            report += $"\n统计信息:\n";
            report += $"总卡片数: {result.totalCards}\n";
            report += $"卡片类型数: {result.uniqueTypes}\n";
            report += $"不规则图形数: {result.totalIrregularShapes}\n";
            
            report += $"\n卡片类型分布:\n";
            foreach (var kvp in result.typeCounts)
            {
                report += $"类型 {kvp.Key}: {kvp.Value} 张\n";
            }
            
            if (result.layerShapeCounts.Count > 0)
            {
                report += $"\n不规则图形层级分布:\n";
                foreach (var kvp in result.layerShapeCounts)
                {
                    report += $"层级 {kvp.Key}: {kvp.Value} 个图形\n";
                }
            }
            
            if (result.errors.Count > 0)
            {
                report += $"\n❌ 错误:\n";
                foreach (var error in result.errors)
                {
                    report += $"  - {error}\n";
                }
            }
            
            if (result.warnings.Count > 0)
            {
                report += $"\n⚠️ 警告:\n";
                foreach (var warning in result.warnings)
                {
                    report += $"  - {warning}\n";
                }
            }
            
            return report;
        }
    }
} 