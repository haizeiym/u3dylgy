using UnityEngine;
using UnityEditor;
using YangLeGeYang2D.LevelEditor;

namespace YangLeGeYang2D.LevelEditor.Editor
{
    public class CuppingLevelEditorMenu
    {
        [MenuItem("拔了个罐/新建关卡", false, 1)]
        public static void NewLevel()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.NewLevel();
                Debug.Log("已创建新关卡");
            }
        }

        [MenuItem("拔了个罐/保存关卡", false, 2)]
        public static void SaveLevel()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.SaveLevel();
            }
        }

        [MenuItem("拔了个罐/加载关卡", false, 3)]
        public static void LoadLevel()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                // 显示关卡选择对话框
                string path = EditorUtility.OpenFilePanel("选择关卡文件", "Assets/script/Levels", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    // 从路径中提取关卡ID
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    if (fileName.StartsWith("Level2D_"))
                    {
                        string idStr = fileName.Substring(8);
                        if (int.TryParse(idStr, out int levelId))
                        {
                            editor.LoadLevel(levelId);
                        }
                    }
                }
            }
        }

        [MenuItem("拔了个罐/导出关卡", false, 4)]
        public static void ExportLevel()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.ExportLevel();
            }
        }

        [MenuItem("拔了个罐/验证关卡", false, 5)]
        public static void ValidateLevel()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.ValidateCurrentLevel();
            }
        }

        [MenuItem("拔了个罐/添加预设图形/圆形", false, 10)]
        public static void AddCircleShape()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.AddPresetShape(0, editor.currentLayer);
                Debug.Log($"已在层级 {editor.currentLayer} 添加圆形图形");
            }
        }

        [MenuItem("拔了个罐/添加预设图形/星形", false, 11)]
        public static void AddStarShape()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.AddPresetShape(1, editor.currentLayer);
                Debug.Log($"已在层级 {editor.currentLayer} 添加星形图形");
            }
        }

        [MenuItem("拔了个罐/添加预设图形/矩形", false, 12)]
        public static void AddRectangleShape()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.AddPresetShape(2, editor.currentLayer);
                Debug.Log($"已在层级 {editor.currentLayer} 添加矩形图形");
            }
        }

        [MenuItem("拔了个罐/添加预设图形/三角形", false, 13)]
        public static void AddTriangleShape()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.AddPresetShape(3, editor.currentLayer);
                Debug.Log($"已在层级 {editor.currentLayer} 添加三角形图形");
            }
        }

        [MenuItem("拔了个罐/清除所有不规则图形", false, 20)]
        public static void ClearAllIrregularShapes()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                editor.irregularShapes.Clear();
                Debug.Log("已清除所有不规则图形");
            }
        }

        [MenuItem("拔了个罐/快速设置", false, 30)]
        public static void QuickSetup()
        {
            var editor = FindOrCreateEditor();
            if (editor != null)
            {
                // 设置默认参数
                editor.gridSize = new Vector2(12, 8);
                editor.cardSpacing = 1.0f;
                editor.cardSize = new Vector2(0.8f, 0.8f);
                editor.totalLayers = 3;
                editor.currentLayer = 0;
                editor.useCustomAreaSize = true;
                editor.areaSize = new Vector2(16f, 12f);
                editor.enableLayerPreview = true;
                editor.showGrid = true;
                editor.debugMode = false;
                editor.highPerformanceMode = true;

                // 添加一些预设图形
                editor.AddPresetShape(0, 0); // 圆形在层级0
                editor.AddPresetShape(1, 1); // 星形在层级1
                editor.AddPresetShape(2, 2); // 矩形在层级2

                Debug.Log("快速设置完成！已添加预设图形到各层级");
            }
        }

        [MenuItem("拔了个罐/关于", false, 100)]
        public static void About()
        {
            EditorUtility.DisplayDialog("拔了个罐2D关卡编辑器", 
                "版本: 1.0.0\n" +
                "功能: 支持不规则图形的2D关卡编辑器\n" +
                "作者: 开发团队\n" +
                "特点:\n" +
                "- 支持多层不规则图形\n" +
                "- 预设图形库\n" +
                "- 实时可视化\n" +
                "- 关卡验证系统\n" +
                "- 集成到script目录", 
                "确定");
        }

        private static CuppingLevelEditor2D FindOrCreateEditor()
        {
            // 查找现有的编辑器
            var existingEditor = Object.FindObjectOfType<CuppingLevelEditor2D>();
            if (existingEditor != null)
            {
                return existingEditor;
            }

            // 创建新的编辑器对象
            GameObject editorObject = new GameObject("拔了个罐2D关卡编辑器");
            var editor = editorObject.AddComponent<CuppingLevelEditor2D>();
            
            // 设置默认参数
            editor.gridSize = new Vector2(10, 10);
            editor.cardSpacing = 1.2f;
            editor.cardSize = new Vector2(1, 1);
            editor.totalLayers = 3;
            editor.currentLayer = 0;
            editor.useCustomAreaSize = true;
            editor.areaSize = new Vector2(16f, 16f);
            editor.enableLayerPreview = true;
            editor.showGrid = true;
            editor.debugMode = false;
            editor.highPerformanceMode = true;

            Debug.Log("已创建拔了个罐2D关卡编辑器对象");
            return editor;
        }
    }
} 