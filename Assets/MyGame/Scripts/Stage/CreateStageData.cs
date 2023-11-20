using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreateStageData
{
    [SerializeField] // マップ情報(地面=0,壁=1)
    private int[,] data = null;
    public int[,] Data => data;
    [SerializeField] // 区画情報
    private List<Rect> rectList = new List<Rect>();
    public List<Rect> RectList => rectList;
    // マップサイズ
    private Vector2Int mapSize = Vector2Int.zero;
    // 部屋の最小サイズ
    private Vector2Int miniSize = Vector2Int.zero;
    // 部屋の最大サイズ
    private Vector2Int maxSize = Vector2Int.zero;
    // 壁の幅
    private int wallWidth = 0;
    // 道の幅
    private int roadWidth = 0;


    public CreateStageData(Vector2Int mapSize, Vector2Int miniSize, Vector2Int maxSize, int wallWidth, int roadWidth)
    {
        this.mapSize = mapSize;
        this.miniSize = miniSize;
        this.maxSize = maxSize;
        this.wallWidth = wallWidth;
        this.roadWidth = roadWidth;

        // データの初期化
        data = new int[mapSize.x, mapSize.y];
        for(int y = 0; y < mapSize.y; y++)
        {
            for(int x = 0; x < mapSize.x; x++)
            {
                data[x,y] = 1;
            }
        }

        // 部屋作成
        CreateRoom(0, mapSize.x - 1, 0, mapSize.y - 1);

        // 道作成
        for(int i = 0; i < rectList.Count; i++)
        {
            Rect rect1 = rectList[i];

            if(rect1.LeftRoadPos != null)
            {
                if(!rect1.LeftRoadPos.IsSet)
                {
                    Rect.RoadPos roadPos1 = rect1.LeftRoadPos;
                    Vector2Int pos1 = new Vector2Int(roadPos1.X, roadPos1.Y);

                    Vector2Int pos2 = Vector2Int.zero;
                    int rand = Random.Range(0, 2);
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];
                        Rect.RoadPos roadPos2 = rect2.RightRoadPos;

                        if(rect1.Left == rect2.Right & 
                            ((rand == 0 & rect2.Up > rect1.Down & rect1.Down >= rect2.Down) | 
                            (rand == 1 & rect2.Up >= rect1.Up & rect1.Up > rect2.Down)))
                        {
                            pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                            Road(pos2, pos1, rect1.Left, true);
                            roadPos1.Set();
                            roadPos2.Set();
                            break;
                        }
                    }
                }
            }
            if(rect1.RightRoadPos != null)
            {
                if(!rect1.RightRoadPos.IsSet)
                {
                    Rect.RoadPos roadPos1 = rect1.RightRoadPos;
                    Vector2Int pos1 = new Vector2Int(roadPos1.X, roadPos1.Y);

                    Vector2Int pos2 = Vector2Int.zero;
                    int rand = Random.Range(0, 2);
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];
                        Rect.RoadPos roadPos2 = rect2.LeftRoadPos;

                        if(rect1.Right == rect2.Left & 
                            ((rand == 0 & rect2.Up > rect1.Down & rect1.Down >= rect2.Down) | 
                            (rand == 1 & rect2.Up >= rect1.Up & rect1.Up > rect2.Down)))
                        {
                            pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                            Road(pos1, pos2, rect1.Right, true);
                            roadPos1.Set();
                            roadPos2.Set();
                            break;
                        }
                    }
                }
            }
            if(rect1.DownRoadPos != null)
            {
                if(!rect1.DownRoadPos.IsSet)
                {
                    Rect.RoadPos roadPos1 = rect1.DownRoadPos;
                    Vector2Int pos1 = new Vector2Int(roadPos1.X, roadPos1.Y);

                    Vector2Int pos2 = Vector2Int.zero;
                    int rand = Random.Range(0, 2);
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];
                        Rect.RoadPos roadPos2 = rect2.UpRoadPos;

                        if(rect1.Down == rect2.Up & 
                            ((rand == 0 & rect2.Right > rect1.Left & rect1.Left >= rect2.Left) | 
                            (rand == 1 & rect2.Right >= rect1.Right & rect1.Right > rect2.Left)))
                        {
                            pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                            Road(pos2, pos1, rect1.Down, false);
                            roadPos1.Set();
                            roadPos2.Set();
                            break;
                        }
                    }
                }
            }
            if(rect1.UpRoadPos != null)
            {
                if(!rect1.UpRoadPos.IsSet)
                {
                    Rect.RoadPos roadPos1 = rect1.UpRoadPos;
                    Vector2Int pos1 = new Vector2Int(roadPos1.X, roadPos1.Y);

                    Vector2Int pos2 = Vector2Int.zero;
                    int rand = Random.Range(0, 2);
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];
                        Rect.RoadPos roadPos2 = rect2.DownRoadPos;

                        if(rect1.Up == rect2.Down & 
                            ((rand == 0 & rect2.Right > rect1.Left & rect1.Left >= rect2.Left) | 
                            (rand == 1 & rect2.Right >= rect1.Right & rect1.Right > rect2.Left)))
                        {
                            pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                            Road(pos1, pos2, rect1.Up, false);
                            roadPos1.Set();
                            roadPos2.Set();
                            break;
                        }
                    }
                }
            }
        }
    }

    // 区画・部屋作成
    public void CreateRoom(int left, int right, int down, int up)
    {
        int Width = right - left + 1;
        int Height = up - down + 1;

        // 分割すか判定
        bool isSplit = false;
        int x = Random.Range(miniSize.x + wallWidth * 2, maxSize.x + wallWidth * 2 + 1);
        int y = Random.Range(miniSize.y + wallWidth * 2, maxSize.y + wallWidth * 2 + 1);
        Vector2Int rectSize = new Vector2Int(x, y);
        if(Width >= rectSize.x * 2 - 1 | Height >= rectSize.y * 2 - 1)
        {
            isSplit = true;
        }

        // 分割する
        if(isSplit)
        {
            // 横に分割するか
            bool isX = true;
            if(Height > Width)
            {
                isX = false;
            }

            // 子供1の広さ
            int size = 0;
            int plusSize = 0;
            if(isX)
            {
                plusSize = Width - (miniSize.x * 2 + 2);
                plusSize = Random.Range((wallWidth * 2 - 2), plusSize - (wallWidth * 2 - 2));
                size = miniSize.x + 1 + plusSize;
            }
            else
            {
                plusSize = Height - (miniSize.y * 2 + 2);
                plusSize = Random.Range((wallWidth * 2 - 2), plusSize - (wallWidth * 2 - 2));
                size = miniSize.y + 1 + plusSize;
            }

            // 分割
            if(isX)
            {
                CreateRoom(left, left + size, down, up);
                CreateRoom(left + size, right, down, up);
            }
            else
            {
                CreateRoom(left, right, down, down + size);
                CreateRoom(left, right, down + size, up);
            }
        }
        // 分割終了
        else
        {
            rectList.Add(new Rect(left, right, down, up, this));
        }
    }

    [System.Serializable] // 区画情報
    public class Rect
    {
        // 位置
        [SerializeField]
        private int left = 0;
        public int Left => left;
        [SerializeField]
        private int right = 0;
        public int Right => right;
        [SerializeField]
        private int down = 0;
        public int Down => down;
        [SerializeField]
        private int up = 0;
        public int Up => up;
        // 広さ
        public int Width => right - left + 1;
        public int Height => up - down + 1;
        [SerializeField] // 部屋
        private Room room = null;
        public Room _Room => room;
        // 道の位置
        [SerializeField]
        private RoadPos leftRoadPos = null;
        public RoadPos LeftRoadPos => leftRoadPos;
        [SerializeField]
        private RoadPos rightRoadPos = null;
        public RoadPos RightRoadPos => rightRoadPos;
        [SerializeField]
        private RoadPos downRoadPos = null;
        public RoadPos DownRoadPos => downRoadPos;
        [SerializeField]
        private RoadPos upRoadPos = null;
        public RoadPos UpRoadPos => upRoadPos;


        public Rect(int left, int right, int down, int up, CreateStageData c)
        {
            this.left = left;
            this.right = right;
            this.down = down;
            this.up = up;

            // 部屋作成
            int xSize = Random.Range(c.miniSize.x, Width - c.wallWidth * 2 + 1);
            int ySize = Random.Range(c.miniSize.y, Height - c.wallWidth * 2 + 1);
            Vector2Int rectSize = new Vector2Int(xSize, ySize);

            int roomLeft = left + c.wallWidth + Random.Range(0, (Width - c.wallWidth * 2) - rectSize.x + 1);
            int roomRight = roomLeft + rectSize.x - 1;
            int roomDown = down + c.wallWidth + Random.Range(0, (Height - c.wallWidth * 2) - rectSize.y + 1);
            int roomUp = roomDown + rectSize.y - 1;

            room = new Room(roomLeft, roomRight, roomDown, roomUp, c);

            // 道路の位置
            int roadPlus = (c.roadWidth - 1) / 2;
            if(left > 0)
            {
                int x = room.Left - 1;
                int y = Random.Range(room.Down + roadPlus, room.Up - roadPlus + 1);
                leftRoadPos = new RoadPos(x, y);
            }
            if(right < c.mapSize.x - 1)
            {
                int x = room.Right + 1;
                int y = Random.Range(room.Down + roadPlus, room.Up - roadPlus + 1);
                rightRoadPos = new RoadPos(x, y);
            }
            if(down > 0)
            {
                int x = Random.Range(room.Left + roadPlus, room.Right - roadPlus + 1);
                int y = room.Down - 1;
                downRoadPos = new RoadPos(x, y);
            }
            if(up < c.mapSize.y - 1)
            {
                int x = Random.Range(room.Left + roadPlus, room.Right - roadPlus + 1);
                int y = room.Up + 1;
                upRoadPos = new RoadPos(x, y);
            }
        }

        [System.Serializable] // 部屋情報
        public class Room
        {
            // 位置
            [SerializeField]
            private int left = 0;
            public int Left => left;
            [SerializeField]
            private int right = 0;
            public int Right => right;
            [SerializeField]
            private int down = 0;
            public int Down => down;
            [SerializeField]
            private int up = 0;
            public int Up => up;
            // 広さ
            public int Width => right - left + 1;
            public int Height => up - down + 1;

            public Room(int left, int right, int down, int up, CreateStageData c)
            {
                this.left = left;
                this.right = right;
                this.down = down;
                this.up = up;

                for(int y = down; y < up + 1; y++)
                {
                    for(int x = left; x < right + 1; x++)
                    {
                        c.data[x,y] = 0;
                    }
                }
            }
        }

        [System.Serializable]
        public class RoadPos
        {
            [SerializeField]
            private int x = 0;
            public int X => x;
            [SerializeField]
            private int y = 0;
            public int Y => y;
            private bool isSet = false;
            public bool IsSet => isSet;

            public RoadPos(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public void Set()
            {
                isSet = true;
            }
        }
    }

    // 道作成
    public void Road(Vector2Int pos1, Vector2Int pos2, int conect, bool isX)
    {
        int roadPlus = (roadWidth - 1) / 2;
        if(isX)
        {
            for(int x = pos1.x; x < conect + roadPlus + 1; x++)
            {
                for(int y = pos1.y - roadPlus; y <= pos1.y + roadPlus; y++)
                {
                    data[x,y] = 0;
                }
            }
            for(int x = conect - roadPlus; x < pos2.x + 1; x++)
            {
                for(int y = pos2.y - roadPlus; y <= pos2.y + roadPlus; y++)
                {
                    data[x,y] = 0;
                }
            }

            int startPos = pos1.y;
            int endPos = pos2.y;
            if(pos1.y > pos2.y)
            {
                startPos = pos2.y;
                endPos = pos1.y;
            }
            for(int y = startPos - roadPlus; y <= endPos + roadPlus; y++)
            {
                for(int x = conect - roadPlus; x <= conect + roadPlus; x++)
                {
                    data[x,y] = 0;
                }
            }
        }
        else
        {
            for(int y = pos1.y; y < conect + roadPlus + 1; y++)
            {
                for(int x = pos1.x - roadPlus; x <= pos1.x + roadPlus; x++)
                {
                    data[x,y] = 0;
                }
            }
            for(int y = conect - roadPlus; y < pos2.y + 1; y++)
            {
                for(int x = pos2.x - roadPlus; x <= pos2.x + roadPlus; x++)
                {
                    data[x,y] = 0;
                }
            }

            int startPos = pos1.x;
            int endPos = pos2.x;
            if(pos1.x > pos2.x)
            {
                startPos = pos2.x;
                endPos = pos1.x;
            }
            for(int x = startPos - roadPlus; x <= endPos + roadPlus; x++)
            {
                for(int y = conect - roadPlus; y <= conect + roadPlus; y++)
                {
                    data[x,y] = 0;
                }
            }
        }
    }
}
