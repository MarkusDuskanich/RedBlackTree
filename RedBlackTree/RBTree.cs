using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBlackTree {

    //------Red Black Tree Conditions------
    //is a BST
    //every node is red or black
    //root is always black
    //all null nodes are black
    //if a node is red then its children are black
    //every path from a node to any of its descendant null nodes has same number of black nodes

    public class RBTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey> {
        private class Node {
            public enum Colors { Red, Black };
            public TKey Key;
            public TValue Value;
            public Node Parent = null;
            public Node LeftChild = null;
            public Node RightChild = null;
            public Colors Color;
        }

        private Node _root = null;


        public TValue this[TKey key] {
            get {
                if (key == null)
                    throw new ArgumentNullException("key can not be null");
                return GetNode(_root, key).Value;
            }
            set {
                if (key == null)
                    throw new ArgumentNullException("key can not be null");
                SetValue(ref _root, null, key, value);
            }
        }

        private void SetValue(ref Node current, Node parent, TKey key, TValue value) {
            //1 new empty node as red, insert
            //2 if new is root, make black and exit
            //3 if parent node is black exit
            //4 if parent is red check color of parents sibling
            //a)    if black then rotate and recolor
            //b)    if red, recolor parent, parent sibling, parent parent and recheck from 2 for grandparent

            if (current == null) {
                //1
                Node newNode = new() {
                    Key = key,
                    Value = value,
                    Parent = parent,
                    Color = Node.Colors.Red
                };
                current = newNode;

                while (true) {
                    //2
                    if (newNode == _root) {
                        newNode.Color = Node.Colors.Black;
                        return;
                    }
                    //3
                    if (!NodeIsRed(parent))
                        return;

                    //4
                    Node uncle = parent.Parent.RightChild == parent ? parent.Parent.LeftChild : parent.Parent.RightChild;
                    if (!NodeIsRed(uncle)) { //a

                        bool isLeftChild = parent.LeftChild == newNode;
                        bool parentIsLeftChild = parent.Parent.LeftChild == parent;
                        if (isLeftChild && parentIsLeftChild) LlRotation(parent.Parent);
                        else if (!isLeftChild && !parentIsLeftChild) RrRotation(parent.Parent);
                        else if (isLeftChild && !parentIsLeftChild) RlRotation(parent.Parent);
                        else if (!isLeftChild && parentIsLeftChild) LrRotation(parent.Parent);
                        return;

                    } else { //b
                        parent.Color = Node.Colors.Black;
                        uncle.Color = Node.Colors.Black;
                        parent.Parent.Color = Node.Colors.Red;
                        newNode = parent.Parent;
                        parent = newNode.Parent;
                    }
                }
            }

            if (current.Key.CompareTo(key) > 0)
                SetValue(ref current.LeftChild, current, key, value);
            else if (current.Key.CompareTo(key) < 0)
                SetValue(ref current.RightChild, current, key, value);
            else
                current.Value = value;
        }

        private Node GetNode(Node current, TKey key) {
            if (current == null)
                throw new KeyNotFoundException($"Key <{key}> does not exist");

            if (current.Key.CompareTo(key) > 0)
                return GetNode(current.LeftChild, key);
            else if (current.Key.CompareTo(key) < 0)
                return GetNode(current.RightChild, key);
            else
                return current;
        }

        public void Remove(TKey key) {
            Delete(GetNode(_root, key));
        }

        private void Delete(Node nodeToDelete) {
            var originalColor = nodeToDelete.Color;
            Node x;
            if (nodeToDelete.LeftChild == null) {
                x = nodeToDelete.RightChild;
                Transplant(x, nodeToDelete);
            }
        }

        private void Transplant(Node origin, Node target) {
            if (origin.Parent == null)
                _root = target;
            else if (origin == target.Parent.LeftChild)
                origin.Parent.LeftChild = target;
                    //fuck lol
        }

        private Node RotateLeft(Node A) {
            Node aParent = A.Parent;
            Node aRightChild = A.RightChild;
            A.RightChild = aRightChild.LeftChild;
            if (aRightChild.LeftChild != null)
                aRightChild.LeftChild.Parent = A;
            aRightChild.LeftChild = A;
            A.Parent = aRightChild;
            aRightChild.Parent = aParent;

            if (aParent != null) {
                if (aParent.RightChild == A)
                    aParent.RightChild = aRightChild;
                else
                    aParent.LeftChild = aRightChild;
            } else
                _root = aRightChild;
            return aRightChild;
        }

        private Node RotateRight(Node A) {
            Node aParent = A.Parent;
            Node aLeftChild = A.LeftChild;
            A.LeftChild = aLeftChild.RightChild;
            if (aLeftChild.RightChild != null)
                aLeftChild.RightChild.Parent = A;
            aLeftChild.RightChild = A;
            A.Parent = aLeftChild;
            aLeftChild.Parent = aParent;

            if (aParent != null) {
                if (aParent.LeftChild == A)
                    aParent.LeftChild = aLeftChild;
                else
                    aParent.RightChild = aLeftChild;
            } else
                _root = aLeftChild;
            return aLeftChild;
        }

        private void LlRotation(Node node) {
            node.Color = Node.Colors.Red;
            node.LeftChild.Color = Node.Colors.Black;
            RotateRight(node);
        }

        private void LrRotation(Node node) {
            node.LeftChild = RotateLeft(node.LeftChild);
            LlRotation(node);
        }

        private void RrRotation(Node node) {
            node.Color = Node.Colors.Red;
            node.RightChild.Color = Node.Colors.Black;
            RotateLeft(node);
        }

        private void RlRotation(Node node) {
            node.RightChild = RotateRight(node.RightChild);
            RrRotation(node);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            List<Node> nodes = new();
            nodes.Add(_root);

            while (nodes.Any()) {
                Node current = nodes.First();
                nodes.RemoveAt(0);

                if (current != null) {
                    yield return KeyValuePair.Create(current.Key, current.Value);
                    nodes.Add(current.LeftChild);
                    nodes.Add(current.RightChild);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private static bool NodeIsRed(Node node) {
            return node?.Color == Node.Colors.Red;
        }

        public void Add(TKey key, TValue value) {
            this[key] = value;
        }

        public void Print() {
            Print(_root, "");
        }

        private void Print(Node current, string connection) {
            if (current == null)
                return;
            Console.Write(connection + (current.Parent?.RightChild == current || current.Parent?.RightChild == null ? "└─" : "├─"));
            if (NodeIsRed(current))
                Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(current.Parent?.LeftChild == current ? "L:" : "R:");
            if (current.Parent == null)
                Console.Write("\rRoot:");
            Console.Write(current.Key + ",");
            Console.WriteLine(current.Value == null ? "null" : current.Value);
            Console.ForegroundColor = ConsoleColor.Gray;
            Print(current.LeftChild, connection + (current.Parent?.LeftChild == current ? "│ " : "  "));
            Print(current.RightChild, connection + (current.Parent?.LeftChild == current ? "│ " : "  "));
        }
    }
}
