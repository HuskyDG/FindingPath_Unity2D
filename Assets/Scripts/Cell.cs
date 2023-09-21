using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : IComparable
{ // một lớp lồng Cell để lưu trữ các thông tin của mỗi ô vuông trên bảng đồ họa
    public int x = -1, y = -1; // vị trí các ô

    // Cho thuật toán mù
    public bool visited = false;
    public bool is_path = false; // đường đi
    public int game_object_type = 0;
    // Dùng cho thuật toán Astar
    public double g = -1, h = -1, f = -1;
    // Dùng cho thuật toán sâu dần
    public int level = -1;

    // Lưu object
    public GameObject game_object = null;

    // Thằng cha của nó để dò đường về nhà (startCell)
    public Cell parent;

    // Hàm khởi tạo
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void resetStat() // reset về trạng thái gốc
    {
        visited = false;
        g = -1; h = -1; f = -1;
        level = -1;
    }

    public void resetEverything()
    {
        resetStat();
        DestroyObject();
        parent = null;
    }

    // Check nếu đường đi có thể di chuyển
    public bool CanBeMovedOn()
    {
        return is_path;
    }

    public int CompareTo(object otherObject)
    {
        Cell otherCell = otherObject as Cell;
        if (this.f > otherCell.f)
            return 1;
        if (this.f < otherCell.f)
            return -1;
        return 0;
    }

    // Hàm phá hủy Object
    public void DestroyObject()
    {
        if (this.game_object == null)
            return;
        this.game_object_type = 0;
        GameObject.Destroy(this.game_object);
        this.game_object = null;
    }
}