using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CreateMap : MonoBehaviour
{
    [SerializeField] // マップサイズ
    private Vector2Int mapSize = Vector2Int.zero;
    [SerializeField] // 部屋の最小サイズ
    private Vector2Int miniSize = Vector2Int.zero;
    [SerializeField] // 外の壁幅
    private int outWallWidth = 0;
    [SerializeField] // 壁の幅
    private int wallWidth = 0;
    [SerializeField] // 道の幅
    private int roadWidth = 0;
    private Vector2Int gap => new Vector2Int(wallWidth*2 + roadWidth, wallWidth*2 + roadWidth);
    
    // マップ情報(地面=0,壁=1)
    private int[,] data = null;
    [SerializeField, ReadOnly] // 区画情報
    private List<Rect> rectList = new List<Rect>();


    public void CreateMapData()
    {
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
        rectList = new List<Rect>();
        CreateRoom(outWallWidth, mapSize.x - outWallWidth - 1, outWallWidth, mapSize.y - outWallWidth - 1);

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

                    Rect.RoadPos roadPos2 = null;
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];

                        if(rect1.Left - gap.x - 1 == rect2.Right)
                        {
                            if(roadPos2 == null)
                            {
                                roadPos2 = rect2.RightRoadPos;
                            }
                            else if(Mathf.Abs(roadPos1.Y - roadPos2.Y) > Mathf.Abs(roadPos1.Y - rect2.RightRoadPos.Y))
                            {
                                roadPos2 = rect2.RightRoadPos;
                            }
                        }
                    }

                    if(roadPos2 != null)
                    {
                        Vector2Int pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                        Road(pos2, pos1, rect1.Left - wallWidth - roadWidth - 1, true);
                        roadPos1.Set();
                        roadPos2.Set();
                    }
                }
            }
            if(rect1.RightRoadPos != null)
            {
                if(!rect1.RightRoadPos.IsSet)
                {
                    Rect.RoadPos roadPos1 = rect1.RightRoadPos;
                    Vector2Int pos1 = new Vector2Int(roadPos1.X, roadPos1.Y);

                    Rect.RoadPos roadPos2 = null;
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];

                        if(rect1.Right + gap.x + 1 == rect2.Left)
                        {
                            if(roadPos2 == null)
                            {
                                roadPos2 = rect2.LeftRoadPos;
                            }
                            else if(Mathf.Abs(roadPos1.Y - roadPos2.Y) > Mathf.Abs(roadPos1.Y - rect2.LeftRoadPos.Y))
                            {
                                roadPos2 = rect2.LeftRoadPos;
                            }
                        }
                    }

                    if(roadPos2 != null)
                    {
                        Vector2Int pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                        Road(pos1, pos2, rect1.Right + wallWidth + 1, true);
                        roadPos1.Set();
                        roadPos2.Set();
                    }
                }
            }
            if(rect1.DownRoadPos != null)
            {
                if(!rect1.DownRoadPos.IsSet)
                {
                    Rect.RoadPos roadPos1 = rect1.DownRoadPos;
                    Vector2Int pos1 = new Vector2Int(roadPos1.X, roadPos1.Y);

                    Rect.RoadPos roadPos2 = null;
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];

                        if(rect1.Down - gap.y - 1 == rect2.Up)
                        {
                            if(roadPos2 == null)
                            {
                                roadPos2 = rect2.UpRoadPos;
                            }
                            else if(Mathf.Abs(roadPos1.X - roadPos2.X) > Mathf.Abs(roadPos1.X - rect2.UpRoadPos.X))
                            {
                                roadPos2 = rect2.UpRoadPos;
                            }
                        }
                    }

                    if(roadPos2 != null)
                    {
                        Vector2Int pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                        Road(pos2, pos1, rect1.Down - wallWidth - roadWidth - 1, false);
                        roadPos1.Set();
                        roadPos2.Set();
                    }
                }
            }
            if(rect1.UpRoadPos != null)
            {
                if(!rect1.UpRoadPos.IsSet)
                {
                    Rect.RoadPos roadPos1 = rect1.UpRoadPos;
                    Vector2Int pos1 = new Vector2Int(roadPos1.X, roadPos1.Y);

                    Rect.RoadPos roadPos2 = null;
                    for(int j = 0; j < rectList.Count; j++)
                    {
                        Rect rect2 = rectList[j];

                        if(rect1.Up + gap.y + 1 == rect2.Down)
                        {
                            if(roadPos2 == null)
                            {
                                roadPos2 = rect2.DownRoadPos;
                            }
                            else if(Mathf.Abs(roadPos1.X - roadPos2.X) > Mathf.Abs(roadPos1.X - rect2.DownRoadPos.X))
                            {
                                roadPos2 = rect2.DownRoadPos;
                            }
                        }
                    }

                    if(roadPos2 != null)
                    {
                        Vector2Int pos2 = new Vector2Int(roadPos2.X, roadPos2.Y);
                        Road(pos1, pos2, rect1.Up + wallWidth + 1, false);
                        roadPos1.Set();
                        roadPos2.Set();
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

        // 横に分割するか
        bool isX = true;
        if(Height - gap.y > Width - gap.x)
        {
            isX = false;
        }

        // 分割すか判定
        bool isSplit = false;
        if(isX)
        {
            if(Width >= miniSize.x*2 + gap.x)
            {
                isSplit = true;
            }
        }
        else
        {
            if(Height >= miniSize.y*2 + gap.y)
            {
                isSplit = true;
            }
        }

        // 分割する
        if(isSplit)
        {
            // 子供1の広さ
            int size = 0;
            int plusSize = 0;
            if(isX)
            {
                plusSize = Random.Range(0, Width - (miniSize.x*2 + gap.x) + 1);
                size = miniSize.x + plusSize;
            }
            else
            {
                plusSize = Random.Range(0, Height- (miniSize.y*2 + gap.y) + 1);
                size = miniSize.y + plusSize;
            }

            // 分割
            if(isX)
            {
                CreateRoom(left, left + size - 1, down, up);
                CreateRoom(left + size + gap.x, right, down, up);
            }
            else
            {
                CreateRoom(left, right, down, down + size - 1);
                CreateRoom(left, right, down + size + gap.y, up);
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


        public Rect(int left, int right, int down, int up, CreateMap c)
        {
            this.left = left;
            this.right = right;
            this.down = down;
            this.up = up;

            // 部屋作成
            int xSize = Random.Range(c.miniSize.x, Width + 1);
            int ySize = Random.Range(c.miniSize.y, Height + 1);
            Vector2Int rectSize = new Vector2Int(xSize, ySize);

            int roomLeft = left + Random.Range(0, Width - rectSize.x + 1);
            int roomRight = roomLeft + rectSize.x - 1;
            int roomDown = down + Random.Range(0, Height - rectSize.y + 1);
            int roomUp = roomDown + rectSize.y - 1;

            room = new Room(roomLeft, roomRight, roomDown, roomUp, c);

            // 道路の位置
            if(left > c.outWallWidth)
            {
                int x = room.Left - 1;
                int y = Random.Range(room.Down, room.Up - c.roadWidth + 1 + 1);
                leftRoadPos = new RoadPos(x, y);
            }
            if(right < c.mapSize.x - c.outWallWidth - 1)
            {
                int x = room.Right + 1;
                int y = Random.Range(room.Down, room.Up - c.roadWidth + 1 + 1);
                rightRoadPos = new RoadPos(x, y);
            }
            if(down > c.outWallWidth)
            {
                int x = Random.Range(room.Left, room.Right - c.roadWidth + 1 + 1);
                int y = room.Down - 1;
                downRoadPos = new RoadPos(x, y);
            }
            if(up < c.mapSize.y - c.outWallWidth - 1)
            {
                int x = Random.Range(room.Left, room.Right - c.roadWidth + 1 + 1);
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

            public Room(int left, int right, int down, int up, CreateMap c)
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
        if(isX)
        {
            for(int x = pos1.x; x < conect + roadWidth; x++)
            {
                for(int y = pos1.y; y < pos1.y + roadWidth; y++)
                {
                    data[x,y] = 0;
                }
            }
            for(int x = conect; x <= pos2.x; x++)
            {
                for(int y = pos2.y; y < pos2.y + roadWidth; y++)
                {
                    data[x,y] = 0;
                }
            }

            int startPos = pos1.y;
            int endPos = pos2.y + roadWidth;
            if(pos1.y > pos2.y)
            {
                startPos = pos2.y;
                endPos = pos1.y + roadWidth;
            }
            for(int y = startPos; y < endPos; y++)
            {
                for(int x = conect; x < conect + roadWidth; x++)
                {
                    data[x,y] = 0;
                }
            }
        }
        else
        {
            for(int y = pos1.y; y < conect + roadWidth; y++)
            {
                for(int x = pos1.x; x < pos1.x + roadWidth; x++)
                {
                    data[x,y] = 0;
                }
            }
            for(int y = conect; y <= pos2.y; y++)
            {
                for(int x = pos2.x; x < pos2.x + roadWidth; x++)
                {
                    data[x,y] = 0;
                }
            }

            int startPos = pos1.x;
            int endPos = pos2.x + roadWidth;
            if(pos1.x > pos2.x)
            {
                startPos = pos2.x;
                endPos = pos1.x + roadWidth;
            }
            for(int x = startPos; x < endPos; x++)
            {
                for(int y = conect; y < conect + roadWidth; y++)
                {
                    data[x,y] = 0;
                }
            }
        }
    }
}
