using System.Collections.Generic;

namespace ReflectionUI
{
    public class UINode
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public UINode parent;

        /// <summary>
        /// 层级索引
        /// </summary>
        public int layer;

        /// <summary>
        /// 所属UI
        /// </summary>
        public BaseView ui;

        /// <summary>
        /// 子节点
        /// </summary>
        public Dictionary<string, UINode> children = new Dictionary<string, UINode>();
    }
}