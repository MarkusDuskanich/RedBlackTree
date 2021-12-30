using System;

namespace RedBlackTree {

    class Program {
        static void Main(string[] args) {
            RBTree<int, Guid> tree = new() {
                { 0, Guid.NewGuid() },
                { 6, Guid.NewGuid() },
                { 9, Guid.NewGuid() },
                { 5, Guid.NewGuid() },
                { 1, Guid.NewGuid() },
            };
            
            //insert some more values
            tree[3] = Guid.NewGuid();
            tree[4] = Guid.NewGuid();
            tree[7] = Guid.NewGuid();
            tree[8] = Guid.NewGuid();
            tree[2] = Guid.NewGuid();
            tree[10] = Guid.NewGuid();


            //traverse tree, breadth first search
            foreach (var keyValuePair in tree) {
                Console.WriteLine($"Value for key <{keyValuePair.Key}> is <{keyValuePair.Value}>");
            }

            //update a value
            tree[5] = Guid.NewGuid();

            Console.WriteLine();

            //display tree in console
            tree.Print();
        }
    }
}
