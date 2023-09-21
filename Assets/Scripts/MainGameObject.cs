using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectPublicState
{

    public int
    SO_HANG = 20, // Khai báo một hằng số SO_HANG để lưu số hàng của bảng đồ họa 
    SO_COT = 20 // Khai báo một hằng số SO_COT để lưu số cột của bảng đồ họa 
    ; // end define

    // giá trị có thể thay đổi bởi object khác
    public Cell[,] GRID; // Không gian
    public Cell startCell, goalCell; // điểm đích
    public int point = -1; // Ghi điểm
}

public class MainGameObject : MonoBehaviour
{
    public GameObjectPublicState st;
    public GameObject coin; // object prefab đồng xu
    public GameObject trap;

    // biến lưu trữ các object nút
    public Text status_txt;
    public Text bfs_btn_txt;
    public Text dfs_btn_txt;
    public Text ifs_btn_txt;
    public Text greedy_btn_txt;
    public Text astar_btn_txt;

    float pos_x, pos_y; // vị trí vật thể đang đứng
    int orig_x, orig_y;
    int x = 0, y = 0; // vị trí trong GRID[row, col];
    int path_index = 0;


    public List<Cell> wpath; // đường đi startCell -> goalCell sau khi giải xong
    public List<Cell> allowPath; // những đường có thể đi
    public int state = 0;

    // Functions

    // Reset các trạng thái
    void ResetGRIDStats()
    {
        for (int row = 0; row < st.SO_HANG; row++)
        { // Duyệt qua từng hàng của bảng đồ họa
            for (int col = 0; col < st.SO_COT; col++)
            { // Duyệt qua từng cột của bảng đồ họa
                st.GRID[row, col].resetStat();
            }
        }
    }

    void ResetEverything()
    {
        for (int row = 0; row < st.SO_HANG; row++)
        { // Duyệt qua từng hàng của bảng đồ họa
            for (int col = 0; col < st.SO_COT; col++)
            { // Duyệt qua từng cột của bảng đồ họa
                st.GRID[row, col].resetEverything();
            }
        }
    }

    // Lấy láng giềng
    public List<Cell> getNeighbors(Cell e)
    { // Khai báo một phương thức getNeighbors để lấy ra các ô vuông láng giềng của một ô vuông cho trước
        int row = e.x; // Lấy ra chỉ số hàng của ô vuông cho trước và gán cho biến row
        int col = e.y; // Lấy ra chỉ số cột của ô vuông cho trước và gán cho biến col
        List<Cell> neighbors = new List<Cell>(); // Khởi tạo một danh sách neighbors là một đối tượng của lớp ArrayList để lưu trữ các ô vuông láng giềng

        if (row - 1 >= 0)
        { // Nếu chỉ số hàng - 1 không âm, tức là có thể di chuyển lên trên được
            neighbors.Add(st.GRID[row - 1, col]); // Thêm ô vuông ở hàng - 1 và cùng cột vào danh sách neighbors
        }
        if (row + 1 < st.SO_HANG)
        { // Nếu chỉ số hàng + 1 nhỏ hơn st.SO_HANG, tức là có thể di chuyển xuống dưới được
            neighbors.Add(st.GRID[row + 1, col]); // Thêm ô vuông ở hàng + 1 và cùng cột vào danh sách neighbors
        }
        if (col - 1 >= 0)
        { // Nếu chỉ số cột - 1 không âm, tức là có thể di chuyển sang trái được
            neighbors.Add(st.GRID[row, col - 1]); // Thêm ô vuông ở cùng hàng và cột - 1 vào danh sách neighbors
        }
        if (col + 1 < st.SO_COT)
        { // Nếu chỉ số cột + 1 nhỏ hơn st.SO_COT, tức là có thể di chuyển sang phải được 
            neighbors.Add(st.GRID[row, col + 1]); // Thêm ô vuông ở cùng hàng và cột + 1 vào danh sách neighbors
        }
        return neighbors; // Trả về danh sách neighbors
    }

    void constructPath(Cell current)
    { // Khai báo một phương thức constructPath để xây dựng đường đi từ st.startCell đến st.goalCell 
        st.point = 0;
        while (current != null)
        { // Lặp cho đến khi ô vuông hiện tại là null, tức là đã quay về st.startCell 
            wpath.Add(current); // Thêm ô vuông hiện tại vào danh sách path 
            //if (current.isFood == true) st.point++;
            current = current.parent; // Gán giá trị của biến parent của ô vuông hiện tại cho biến current, tức là di chuyển ngược lại quan hệ cha con trong quá trình duyệt BFS 
        }
        wpath.Reverse(); // Đảo ngược danh sách path để có thứ tự từ st.startCell đến st.goalCell
    }

    // Giải thuật
    public enum resolve_method
    {
        BFS_RESOLVE, // Chiều rộng
        DFS_RESOLVE, // Chiều sâu
        IFS_RESOLVE, // Sâu dần
        GREEDY_RESOLVE, //ham ăn
        ASTAR_RESOLVE // A*
    };

    public resolve_method resolve_mask = resolve_method.ASTAR_RESOLVE;

    void resolve(resolve_method num)
    {
        string txt = "";
        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
        Text btn_txt = null;
        if (num == resolve_method.BFS_RESOLVE)
        {
            txt = "Solve BFS";
            btn_txt = bfs_btn_txt;
            solveBFS();
        }
        else if (num == resolve_method.DFS_RESOLVE)
        {
            txt = "Solve DFS";
            btn_txt = dfs_btn_txt;
            solveDFS();
        }
        else if (num == resolve_method.IFS_RESOLVE)
        {
            txt = "Solve IFS";
            btn_txt = ifs_btn_txt;
            solveIFS();
        }
        else if (num == resolve_method.ASTAR_RESOLVE)
        {
            txt = "Solve A*";
            btn_txt = astar_btn_txt;
            solveAstar();
        }
        else if (num == resolve_method.GREEDY_RESOLVE)
        {
            txt = "Solve Greedy";
            btn_txt = greedy_btn_txt;
            solveGreedy();
        }
        sw.Stop();
        btn_txt.text = txt + " (" + sw.ElapsedMilliseconds + " ms) ";

        Debug.Log("Đã giải xong!");
        // Reset các trạng thái
        ResetGRIDStats();
    }

    private void solveBFS()
    { // Khai báo một phương thức solveBFS để giải quyết trò chơi bằng thuật toán BFS
        Queue<Cell> _queue = new Queue<Cell>(); // Khởi tạo một hàng đợi queue là một đối tượng của lớp LinkedList để lưu trữ các ô vuông cần duyệt
        _queue.Enqueue(st.startCell); // Thêm st.startCell vào hàng đợi
        st.startCell.visited = true; // Đặt giá trị true cho biến visited của st.startCell, tức là đã được duyệt

        while (_queue.Count != 0)
        { // Lặp cho đến khi hàng đợi rỗng
            Cell current = _queue.Dequeue(); // Lấy ra và xóa phần tử đầu tiên của hàng đợi và gán cho biến current
            Debug.Log("Queue: " + current.x + ":" + current.y);

            if (current == st.goalCell)
            { // Nếu ô vuông hiện tại là st.goalCell
                constructPath(current); // Gọi phương thức constructPath với tham số là ô vuông hiện tại để xây dựng đường đi từ st.startCell đến st.goalCell
                break; // Thoát khỏi vòng lặp
            }

            foreach (Cell neighbor in getNeighbors(current))
            { // Duyệt qua từng ô vuông láng giềng của ô vuông hiện tại bằng cách gọi phương thức getNeighbors với tham số là ô vuông hiện tại
                if (!neighbor.visited && neighbor.CanBeMovedOn())
                { // Nếu ô vuông láng giềng chưa được duyệt và không phải là chướng ngại vật
                    _queue.Enqueue(neighbor); // Thêm ô vuông láng giềng vào hàng đợi
                    neighbor.visited = true; // Đặt giá trị true cho biến visited của ô vuông láng giềng, tức là đã được duyệt
                    neighbor.parent = current; // Đặt giá trị của biến parent của ô vuông láng giềng là ô vuông hiện tại, tức là lưu lại quan hệ cha con trong quá trình duyệt BFS
                }
            }
        }
    }

    private void solveDFS()
    {
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(st.startCell);
        st.startCell.visited = true;

        while (stack.Count != 0)
        { // Lặp cho đến khi ngăn xếp rỗng
            Cell current = stack.Pop(); // Lấy ra và xóa phần tử đầu tiên của ngăn xếp và gán cho biến current
            Debug.Log("Queue: " + current.x + ":" + current.y);

            if (current == st.goalCell)
            { // Nếu ô vuông hiện tại là st.goalCell
                constructPath(current); // Gọi phương thức constructPath với tham số là ô vuông hiện tại để xây dựng đường đi từ st.startCell đến st.goalCell
                break; // Thoát khỏi vòng lặp
            }

            foreach (Cell neighbor in getNeighbors(current))
            { // Duyệt qua từng ô vuông láng giềng của ô vuông hiện tại bằng cách gọi phương thức getNeighbors với tham số là ô vuông hiện tại
                if (!neighbor.visited && neighbor.CanBeMovedOn())
                { // Nếu ô vuông láng giềng chưa được duyệt và không phải là chướng ngại vật
                    stack.Push(neighbor); // Đẩy ô vuông láng giềng vào ngăn xếp
                    neighbor.visited = true; // Đặt giá trị true cho biến visited của ô vuông láng giềng, tức là đã được duyệt
                    neighbor.parent = current; // Đặt giá trị của biến parent của ô vuông láng giềng là ô vuông hiện tại, tức là lưu lại quan hệ cha con trong quá trình duyệt DFS
                }
            }
        }
    }

    private bool solveDLS(int limit)
    {
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(st.startCell);
        st.startCell.visited = true;
        st.startCell.level = 0;

        while (stack.Count != 0)
        { // Lặp cho đến khi ngăn xếp rỗng
            Cell current = stack.Pop(); // Lấy ra và xóa phần tử đầu tiên của ngăn xếp và gán cho biến current
            Debug.Log("Queue: " + current.x + ":" + current.y);

            if (current == st.goalCell)
            { // Nếu ô vuông hiện tại là st.goalCell
                constructPath(current); // Gọi phương thức constructPath với tham số là ô vuông hiện tại để xây dựng đường đi từ st.startCell đến st.goalCell
                return true; // Thoát khỏi vòng lặp
            }

            if (current.level >= limit)
            {
                return false;
            }

            foreach (Cell neighbor in getNeighbors(current))
            { // Duyệt qua từng ô vuông láng giềng của ô vuông hiện tại bằng cách gọi phương thức getNeighbors với tham số là ô vuông hiện tại
                if (!neighbor.visited && neighbor.CanBeMovedOn())
                { // Nếu ô vuông láng giềng chưa được duyệt và không phải là chướng ngại vật
                    stack.Push(neighbor); // Đẩy ô vuông láng giềng vào ngăn xếp
                    neighbor.visited = true; // Đặt giá trị true cho biến visited của ô vuông láng giềng, tức là đã được duyệt
                    neighbor.parent = current; // Đặt giá trị của biến parent của ô vuông láng giềng là ô vuông hiện tại, tức là lưu lại quan hệ cha con trong quá trình duyệt DFS
                    neighbor.level = current.level + 1;
                }
            }
        }
        return false;
    }

    private void solveIFS()
    {
        // sâu dần
        for (int limit = 0; limit < st.SO_HANG * st.SO_COT && !solveDLS(limit); limit++)
            ResetGRIDStats();
    }

    private double cost(Cell p, Cell q)
    {
        return Math.Sqrt(Math.Pow(p.x - q.x, 2) + Math.Pow(p.y - q.y, 2));
    }

    private Cell PickSmallestCell(List<Cell> list)
    {
        if (list.Count == 0) return null;
        Cell min = list[0];
        int min_index = 0;
        for (int i = 1; i < list.Count; i++)
        {
            if (min.CompareTo(list[i]) > 0) {
                min = list[i];
                min_index = 0;
            }
        }
        list.RemoveAt(min_index);
        return min;
    }

    private void solveAstar()
    {
        // Create a priority queue of cells with integer priorities
        List<Cell> openSet = new List<Cell>();
        // Create a hash set of cells to store the closed set
        HashSet<Cell> closedSet = new HashSet<Cell>();

        st.startCell.g = 0;
        st.startCell.h = cost(st.startCell, st.goalCell);
        st.startCell.f = st.startCell.g + st.startCell.h;
        // Add the start cell to the open set with its f value as the priority
        openSet.Add(st.startCell);

        while (openSet.Count != 0)
        {
            openSet.Sort();
            // Remove and return the cell with the highest priority from the open set
            Cell current = PickSmallestCell(openSet);

            Debug.Log("Queue: " + current.x + ":" + current.y);

            if (current == st.goalCell)
            {
                constructPath(current);
                return;
            }
            // Add the current cell to the closed set
            closedSet.Add(current);

            foreach (Cell neighbor in getNeighbors(current))
            { // Loop through each neighbor cell of the current cell by calling the getNeighbors method with the current cell as the parameter
                if (!neighbor.CanBeMovedOn()) // Skip obstacles
                    continue;
                // If the neighbor cell is not visited and not an obstacle
                double tenG = current.g + 1;
                if (closedSet.Contains(neighbor) && tenG >= neighbor.g)
                    continue;
                if (!openSet.Contains(neighbor) || tenG < neighbor.g)
                {
                    neighbor.parent = current;
                    neighbor.g = tenG;
                    neighbor.h = cost(neighbor, st.goalCell);
                    neighbor.f = neighbor.g + neighbor.h;
                    // Add the neighbor cell to the open set with its f value as the priority
                    openSet.Add(neighbor);
                }
                neighbor.visited = true;
            }
        }

    }

    private void solveGreedy()
    {
        // Create a priority queue of cells with integer priorities
        List<Cell> openSet = new List<Cell>(); 
        // Create a hash set of cells to store the closed set
        HashSet<Cell> closedSet = new HashSet<Cell>();

        st.startCell.h = cost(st.startCell, st.goalCell);
        // Add the start cell to the open set with its h value as the priority
        openSet.Add(st.startCell);

        while (openSet.Count != 0)
        {
            openSet.Sort();
            // Remove and return the cell with the highest priority from the open set
            Cell current = PickSmallestCell(openSet);

            Debug.Log("Queue: " + current.x + ":" + current.y);

            if (current == st.goalCell)
            {
                constructPath(current);
                return;
            }
            // Add the current cell to the closed set
            closedSet.Add(current);

            foreach (Cell neighbor in getNeighbors(current))
            { // Loop through each neighbor cell of the current cell by calling the getNeighbors method with the current cell as the parameter
                if (!neighbor.CanBeMovedOn()) // Skip obstacles
                    continue;
                // If the neighbor cell is not visited and not an obstacle
                if (closedSet.Contains(neighbor))
                    continue;
                if (!openSet.Contains(neighbor))
                {
                    neighbor.parent = current;
                    neighbor.h = cost(neighbor, st.goalCell);
                    // Add the neighbor cell to the open set with its h value as the priority
                    openSet.Add(neighbor);
                }
                neighbor.visited = true;
            }
        }

    }

    private void make_path(int x1, int y1, int x2, int y2)
    {
        for (int row = x1; row <= x2; row++)
            for (int col = y1; col <= y2; col++)
            {
                if (st.GRID[row, col].is_path)
                    return;
                st.GRID[row, col].is_path = true;
                allowPath.Add(st.GRID[row, col]);
            }
    }

    private bool is_valid_pos()
    {
        return (pos_x >= 0 && pos_x <= st.SO_HANG - 1) &&
                (pos_y >= 0 && pos_y <= st.SO_COT - 1);
    }

    // Hàm lấy vị trí ngẫu nhiên trên đường có thể đi
    Cell getRandomCellOnPath()
    {
        int rand = UnityEngine.Random.Range(0, allowPath.Count - 1);
        return allowPath[rand];
    }

    // 
    public void NewGame()
    {
        if (state != 0)
            return;

        ResetEverything();

        // Khởi tạo vị trí ngẫu nhiên của 10 đồng xu trên đường đi
        for (int i = 0; i < 30; i++)
        {
            Cell rand_cell = getRandomCellOnPath();
            if (rand_cell.game_object_type == 0)
            { // Khởi tạo bẫy và rải vào bản đồ
                rand_cell.game_object_type = 1;
                rand_cell.game_object = Instantiate(coin); // clone object
                rand_cell.game_object.SetActive(true);
                rand_cell.game_object.transform.position = new Vector3(rand_cell.x, rand_cell.y, -3); // Di chuyển vào đúng ô
                rand_cell.game_object.name = "Money " + rand_cell.x + ":" + rand_cell.y; // Đặt tên
            }
        }

        // Rải vài bẫy
        for (int i = 0; i < 10; i++)
        {
            Cell rand_cell = getRandomCellOnPath();
            if (rand_cell.game_object_type == 0)
            { // Khởi tạo bẫy và rải vào bản đồ
                rand_cell.game_object_type = 2;
                rand_cell.game_object = Instantiate(trap); // clone object
                rand_cell.game_object.SetActive(true);
                rand_cell.game_object.transform.position = new Vector3(rand_cell.x, rand_cell.y, -3); // Di chuyển vào đúng ô
                rand_cell.game_object.name = "Trap " + rand_cell.x + ":" + rand_cell.y; // Đặt tên
            }
        }


        Cell random_start = getRandomCellOnPath();
        orig_x = random_start.x;
        orig_y = random_start.y;
        x = orig_x;
        y = orig_y;

        //khởi tạo mặc định
        st.startCell = st.GRID[x, y];
        st.goalCell = st.GRID[19, 19];

        // Di chuyển object về ô bắt đầu
        transform.position = new Vector3(orig_x, orig_y, -3);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Khởi tạo trạng thái
        st = new GameObjectPublicState();

        pos_x = (float)transform.position.x;
        pos_y = (float)transform.position.y;
        if (!is_valid_pos())
        {
            gameObject.SetActive(false);
        }

        Debug.Log("Start!");
        // Khởi tạo không gian trạng thái
        st.GRID = new Cell[st.SO_HANG, st.SO_COT];
        for (int row = 0; row < st.SO_HANG; row++)
        { // Duyệt qua từng hàng của bảng đồ họa
            for (int col = 0; col < st.SO_COT; col++)
            { // Duyệt qua từng cột của bảng đồ họa
                st.GRID[row, col] = new Cell(row, col); // Khởi tạo mỗi ô vuông trong bảng đồ họa là một đối tượng của lớp Cell với chỉ số hàng và cột tương ứng
            }
        }


        // Khởi tạo path
        wpath = new List<Cell>();
        allowPath = new List<Cell>(); // để ghi nhớ những đường có thể đi

        // Khởi tạo đường đi
        make_path(0, 0, 19, 0);
        make_path(0, 1, 0, 13);
        make_path(1, 13, 9, 13);
        make_path(1, 5, 6, 5);
        make_path(6, 1, 6, 4);
        make_path(0, 19, 19, 19);
        make_path(0, 16, 0, 18);
        make_path(1, 16, 5, 16);
        make_path(5, 14, 5, 15);
        make_path(1, 9, 11, 9);
        make_path(14, 1, 14, 3);
        make_path(11, 3, 13, 3);
        make_path(8, 4, 11, 4);
        make_path(11, 5, 11, 8);
        make_path(19, 1, 19, 6);
        make_path(14, 6, 18, 6);
        make_path(14, 7, 14, 11);
        make_path(9, 13, 9, 18);
        make_path(7, 10, 7, 13);
        make_path(15, 11, 16, 11);
        make_path(17, 9, 17, 11);
        make_path(17, 9, 19, 9);
        make_path(12, 14, 19, 14);
        make_path(12, 15, 12, 18);
        make_path(19, 9, 19, 18);

        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == 0)
        {
            return;
        }
        if (state == 1)
        {
            transform.position = new Vector3(orig_x, orig_y, transform.position.z);
            x = orig_x;
            y = orig_y;
            path_index = 0;
            wpath.Clear();
            // Thực hiện giải thuật
            resolve(resolve_mask);
            state++; // state 2
        }

        // lấy vị trí của Main Object
        pos_x = (float)transform.position.x;
        pos_y = (float)transform.position.y;


        Debug.Log("Hiện tại: " + pos_x + ":" + pos_y);
        status_txt.text = "Điểm hiện tại: " + st.point;

        if ((st.goalCell.parent == null) || (Math.Round(pos_x * 1000f) == Math.Round(st.goalCell.x * 1000f) &&
            Math.Round(pos_y * 1000f) == Math.Round(st.goalCell.y * 1000f)))
        {
            state = 0; // reset state
            path_index = 0;
            wpath.Clear();
            Debug.Log("Đã đi đến đích hay không tìm thấy đường đi!");
            return;
        }

        // Chuẩn bị di chuyển
        x = wpath[path_index].x;
        y = wpath[path_index].y;

        // Nếu đi vào ô tiền, cập nhật điểm +1
        if (st.GRID[x, y].game_object_type == 1)
        {
            st.GRID[x, y].DestroyObject(); // Phá hủy Object
            st.point++;
        }

        if ((Math.Round(pos_x * 1000f) == Math.Round(x * 1000f)) &&
            (Math.Round(pos_y * 1000f) == Math.Round(y * 1000f))
            && st.GRID[x, y].game_object_type == 2)
        {
            st.point--;
        }

        int next_x = wpath[path_index + 1].x;
        int next_y = wpath[path_index + 1].y;

        if (Math.Round(pos_x * 1000f) == Math.Round(next_x * 1000f) &&
            Math.Round(pos_y * 1000f) == Math.Round(next_y * 1000f))
        {
            path_index++;
            return;
        }

        Debug.Log("Bước đi: " + x + ":" + y + " --> " + next_x + ":" + next_y);


        // Cập nhật di chuyển
        Vector3 new_position = new Vector3(next_x, next_y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, new_position, 0.02f);
    }
}

