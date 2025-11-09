using System;
using System.Collections.Generic;

public class AVLNode
{
    public double Key;           
    public string[] RowData;     
    public int Height;
    public AVLNode Left, Right;

    public AVLNode(double key, string[] row)
    {
        Key = key;
        RowData = row;
        Height = 1;
    }
}

public class AVLTree
{
    public AVLNode Root;

    int Height(AVLNode N) => N?.Height ?? 0;

    int GetBalance(AVLNode N) => N == null ? 0 : Height(N.Left) - Height(N.Right);

    AVLNode RightRotate(AVLNode y)
    {
        AVLNode x = y.Left;
        AVLNode T2 = x.Right;

        x.Right = y;
        y.Left = T2;

        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

        return x;
    }

    AVLNode LeftRotate(AVLNode x)
    {
        AVLNode y = x.Right;
        AVLNode T2 = y.Left;

        y.Left = x;
        x.Right = T2;

        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

        return y;
    }

    public AVLNode Insert(AVLNode node, double key, string[] row)
    {
        if (node == null)
            return new AVLNode(key, row);

        if (key <= node.Key)
            node.Left = Insert(node.Left, key, row);
        else if (key > node.Key)
            node.Right = Insert(node.Right, key, row);
        

        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

        int balance = GetBalance(node);

        // 4 trường hợp mất cân bằng
        if (balance > 1 && key <= node.Left.Key)
            return RightRotate(node);

        if (balance < -1 && key > node.Right.Key)
            return LeftRotate(node);

        if (balance > 1 && key > node.Left.Key)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        if (balance < -1 && key < node.Right.Key)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }
    public List<AVLNode> InOrder(AVLNode node)
    {
        List<AVLNode> list = new List<AVLNode>();
        if (node == null) return list;

        list.AddRange(InOrder(node.Left));
        list.Add(node);
        list.AddRange(InOrder(node.Right));

        return list;
    }
    //Tính chiều cao cây 
    public int GetTreeHeight(AVLNode node)
    {
        if (node == null) return 0;
        int leftHeight = GetTreeHeight(node.Left);
        int rightHeight = GetTreeHeight(node.Right);
        return 1 + Math.Max(leftHeight, rightHeight);
    }

    // Đếm số node lá
    public int CountLeafNodes(AVLNode node)
    {
        if (node == null) return 0;
        if (node.Left == null && node.Right == null) return 1;
        return CountLeafNodes(node.Left) + CountLeafNodes(node.Right);
    }

    // Tìm giá trị nhỏ nhất 
    public double FindMin(AVLNode node)
    {
        if (node == null)
            throw new InvalidOperationException("Cây rỗng!");
        if (node.Left == null)
            return node.Key;
        return FindMin(node.Left);
    }

    // Tìm giá trị lớn nhất 
    public double FindMax(AVLNode node)
    {
        if (node == null)
            throw new InvalidOperationException("Cây rỗng!");
        if (node.Right == null)
            return node.Key;
        return FindMax(node.Right);
    }

    // Tìm giá trị x 
    public AVLNode Search(AVLNode node, double key)
    {
        if (node == null) return null;
        if (key == node.Key)
            return node;
        else if (key < node.Key)
            return Search(node.Left, key);
        else
            return Search(node.Right, key);
    }
    public List<AVLNode> SearchAll(AVLNode node, double key)
    {
        List<AVLNode> result = new List<AVLNode>();
        if (node == null)
            return result;

        // Duyệt trái
        result.AddRange(SearchAll(node.Left, key));

        // So sánh giá trị
        if (node.Key == key)
            result.Add(node);

        // Duyệt phải
        result.AddRange(SearchAll(node.Right, key));

        return result;
    }
    public List<AVLNode> GetNodesAtLevel(AVLNode node, int level)
    {
        List<AVLNode> result = new List<AVLNode>();
        GetNodesAtLevelRecursive(node, level, 1, result);
        return result;
    }

    private void GetNodesAtLevelRecursive(AVLNode node, int targetLevel, int currentLevel, List<AVLNode> list)
    {
        if (node == null) return;
        if (currentLevel == targetLevel)
        {
            list.Add(node);
            return;
        }
        GetNodesAtLevelRecursive(node.Left, targetLevel, currentLevel + 1, list);
        GetNodesAtLevelRecursive(node.Right, targetLevel, currentLevel + 1, list);
    }


}
