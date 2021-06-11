using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MathRoom.Helpers.Structs
{
    public struct TreeNode<T>
    {
        private readonly T _value;
        private readonly List<TreeNode<T>> _children;

        public TreeNode(T value)
        {
            _value = value;
            _children = new List<TreeNode<T>>();
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public T Value { get { return _value; } }

        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value);
            _children.Add(node);
            return node;
        }

        public void AddChild(TreeNode<T> _node)
        {
            _children.Add(_node);
        }

        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(AddChild).ToArray();
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public IEnumerable<T> Flatten()
        {
            return new[] {Value}.Concat(_children.SelectMany(x => x.Flatten()));
        }
    }
}