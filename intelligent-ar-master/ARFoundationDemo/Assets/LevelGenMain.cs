using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Cell
{
    public GameObject indicator;
    public Vector3 position { get; set; }
    public Edge upNeighborEdge { get; set; }
    public Edge downNeighborEdge { get; set; }
    public Edge leftNeighborEdge { get; set; }
    public Edge rightNeighborEdge { get; set; }
    public float g { get; set; }
    public float h { get; set; }
    public Cell prev { get; set; }

    public List<Edge> jumpReachableNeighborEdges { get; set; }

    public Cell(Vector3 pos)
    {
        position = pos;
        upNeighborEdge = null;
        downNeighborEdge = null;
        leftNeighborEdge = null;
        rightNeighborEdge = null;
        jumpReachableNeighborEdges = new List<Edge>();

        g = Mathf.Infinity;
        h = Mathf.Infinity;
        prev = null;
    }
}

public class Edge
{
    public Cell dest { get; set; }
    public float weight { get; set; }

    public Edge()
    {
        dest = null;
        weight = 1.0f;
    }

    public Edge (Cell destCell)
    {
        dest = destCell;
        weight = 1.0f;
    }
}

public class LevelGenMain : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text Log1Text;
    [SerializeField] TMPro.TMP_Text Log2Text;
    [SerializeField] TMPro.TMP_Text Log3Text;
    [SerializeField] TMPro.TMP_Text Log4Text;

    public ARGameObject startPlatform;

    public GameObject cellPointPrefab;
    public GameObject cell;
	public GameObject lineRendererObj;

	public GameObject loggerObj;
    private Logging logger;

    int numUpdates = 0;
    bool isGridifyDone = false;

    private JumpArcCalculator jac;

    private List<ARGameObject> platforms;

    private JsonParser prsr;

    private ARGameObject endPlatfrom;

    // Start is called before the first frame update
    void Start()
    {
        logger = loggerObj.GetComponent<Logging>();
        logger.Log("Started Start LevelGenMain");
        jac = GetComponent<JumpArcCalculator>();
        // figure out where Start and End platforms are
        // get the ARPlatforms list from GameParse.cs
        platforms = GameParse.getARPlatforms();

        logger.Log("just AR platforms #: " + platforms.Count);

        // get the platform farthest from the start platform
        (ARGameObject, float) maxDistPlatformInfo = (null, -1.0f);
        float currMaxDist = -1.0f;
        ARGameObject currMaxPlatform = null;
        for (int i = 0; i < platforms.Count; i++)
        {
            currMaxPlatform = platforms[i];
            currMaxDist = Vector3.Distance(startPlatform.go.transform.position, currMaxPlatform.go.transform.position);
            if (currMaxDist > maxDistPlatformInfo.Item2)
            {
                maxDistPlatformInfo.Item1 = currMaxPlatform;
                maxDistPlatformInfo.Item2 = currMaxDist;
            }
        }

        Log3Text.text = "Got the farthest platform";

        endPlatfrom = maxDistPlatformInfo.Item1;

        // generate the platforms
        // get reference to list of arcs from arc calculator
        List<List<GameObject>> arcs = jac.arcPointList;
        // start from the start platform and see which downward point in the arc is closest to the other end platform
        bool gotToEndPlatform = false;
        ARGameObject pltfrm = startPlatform;
        GameObject defaultPlatform = GetComponent<JsonParser>().defaultPlane;
        prsr = GetComponent<JsonParser>();
        Log3Text.text = "About to start generating platforms";

        while (!gotToEndPlatform)
        {
            // todo: nice to have: check that the platform does not touch any of the upward points in that arc

            // find the arc point closest to end platform
            GameObject closestPoint = null;
            //try
            //{
            closestPoint = findClosestDownwardPoint(pltfrm.arcs, endPlatfrom.go);
            /*}
            catch(Exception e)
            {
                var LineNumber = new StackTrace(e, true).GetFrame(0).GetFileLineNumber();
                Log3Text.text = "Exception: " + LineNumber;
            }*/
            //Log3Text.text = "found closest point";
            if (closestPoint != null)
            {
                // Instantiate a platform there
                ARGameObject aPlatform = prsr.parsePlatform();
                aPlatform.setAngle(0.0, 0.0);
                Log3Text.text = "made a platform";

                // add thtis platform to the platforms list
                platforms.Add(aPlatform);

                Log3Text.text = "closest point is NOT null";
                // set the location of this platform
                aPlatform.go.transform.position = closestPoint.transform.position;
                // place the arcs on new platform and start process over again
                aPlatform.arcs = jac.setArcsGen(aPlatform.go, arcs);
                /*List<Vector3> edgePoints = jac.getEdgePoints(new GameObject[] { aPlatform.go }, jac.createRay(new Vector3(0, 0, 0), new Vector3(0, -2, 0)), 8, 2);
                Log3Text.text = "# edge points: " + edgePoints.Count.ToString();
                aPlatform.arcs = jac.placeJumpArcsAtEdges(arcs, edgePoints);
                */
                //Log3Text.text = "placed the jump arcs";
                // set pltfrm
                pltfrm = aPlatform;
            }
            else
            {
                // one of the arcs is hitting the end platform so no need to generate another
                gotToEndPlatform = true;
            }

            logger.Log("# platforms after added another generated one or finished: " + platforms.Count);
        }

        // delete the prefab arc also
        jac.deleteArcCluster(jac.arcPointList);

        // delete the prefab platform and vertical surface also
        //Destroy(GameParse);
        Destroy(GameParse.getVertSurface().go);

        Log3Text.text = "Completed Start LevelGenMain";

    }

    void gridifyPlatforms(List<ARGameObject> platforms, float stepSize)
    {

        try
        {
            int touchMask = LayerMask.GetMask("touchable");
            int numPlats = platforms.Count;
            logger.Log("num platforms: " + numPlats);
            for (int i = 0; i < numPlats; i++)
            {
                ARGameObject currARPlat = platforms[i];
                GameObject currPlat = currARPlat.go;
                Collider currPlatCol = currPlat.GetComponent<Collider>();
                logger.Log("platform object: " + currPlat.transform.position);

                // todo: get height of this collider and be above that
                // calculate origin
                Vector3 origin = currPlatCol.transform.position + Vector3.up;

                // debug points
                /*GameObject currCellPointPref = Instantiate(cellPointPrefab);
                GameObject endPointPref = Instantiate(cellPointPrefab);
                currCellPointPref.transform.position = origin;
                endPointPref.transform.position = origin - new Vector3(0, 5, 0);
                */

                Vector3 dir = (origin - new Vector3(0,5,0)) - origin;

                Ray ray = new Ray(origin, dir);
                RaycastHit hit;
                currPlatCol.Raycast(ray,out hit, Mathf.Infinity);

                logger.Log(" === iteration " + i + " ===");
            }
        }
        catch(Exception e)
        {
            logger.Log("Exception in gridifyPlatforms: " + e.ToString());
        }
        
    }

    GameObject findClosestDownwardPoint(List<List<List<GameObject>>> arcs, GameObject endPlatform)
    {
        Log3Text.text = "Starting findClosestDownwardPoint";
        // todo: also think about instead finding one with smallest angle in the x,z plane instead. This can maybe take out the inner for loop maybe
        (GameObject, float) prevMinDistInfo = (null, -1);
        for (int i = 0; i < arcs.Count; i++)
        {
            List<List<GameObject>> arcCluster = arcs[i];
            int arcClusterLen = arcCluster.Count;
            Log3Text.text = "Got an arcCluster";
            for (int j = 0; j < arcClusterLen; j++)
            {
                List<GameObject> anArc = arcCluster[j];
                GameObject prevPoint = null;
                Log3Text.text = "Got an arc";
                int singleArcLen = anArc.Count;
                for (int k = 0; k < singleArcLen; k++)
                {
                    GameObject point = anArc[k];
                    Log3Text.text = "Got a point";

                    // check if this piece of the arc touches the end platform
                    if (prevPoint != null && intersectsPlatform(endPlatform, prevPoint, point))
                    {
                        Log3Text.text = "returning null";
                        // one of the arcs is hitting the end platform so no need to generate another
                        return null;
                    }

                    Log3Text.text = "Called/skipped intersectsPlatform";

                    if (prevPoint != null)
                    {
                        // check if the point y is < previous y
                        Vector3 prevPos = prevPoint.transform.position;
                        Vector3 currPos = point.transform.position;
                        Log3Text.text = "Got the previous and curr positions";
                        if (prevPos.y > currPos.y)
                        {
                            Log3Text.text = "Found a downward point";
                            // if so, get the distance between this point and the end platform
                            float dist = Vector3.Distance(currPos, endPlatform.transform.position);

                            // if smaller than smallest thus far, save that value
                            if (prevMinDistInfo.Item1 == null || dist < prevMinDistInfo.Item2)
                            {
                                Log3Text.text = "Found a smallest distance thus far";
                                prevMinDistInfo = (point, dist);
                            }
                        }
                    }
                    prevPoint = point;
                }

            }
        }
        Log3Text.text = "returning smallest dist";
        return prevMinDistInfo.Item1;
    }

    bool intersectsPlatform(GameObject aPlatform, GameObject pt1, GameObject pt2)
    {
        Vector3 pos1 = pt1.transform.position;
        Vector3 pos2 = pt2.transform.position;
        float dist = Vector3.Distance(pt1.transform.position, pt2.transform.position);
        Vector3 unitVec = (pos2 - pos1).normalized;

        RaycastHit rh = raycastHitWhich(pos1, unitVec, dist);
        if (rh.collider == aPlatform.GetComponent<Collider>())
        {
            // the arc hit the given platform
            return true;
        }
        // did not hit
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        // call gridifyPlatforms only once here then never again
        if (!isGridifyDone)
        {
            //gridifyPlatforms(platforms);
            try
            {
                float stepSize = prsr.getAvatar().stepSize;
                int len = platforms.Count;
                for (int i = 0; i < len; i++)
                {
                    List<List<Cell>> bigGrid = getBigGrid(platforms[i].go, stepSize);
                    showBigGrid(bigGrid, cellPointPrefab);

                    logger.Log("+++++++++++++++++++++++++++++++++++++++++");

                    bigGrid = gridifyPlatform(platforms[i].go, bigGrid);
                    platforms[i].grid = bigGrid;
                    //showBigGrid(platforms[i].grid, cell);

                    logger.Log("#########################################");
                }
            }
            catch (Exception e)
            {
                logger.Log(e.ToString());
            }

            try
            {
                setAllJumpNeighbors();
                int len = platforms.Count;
                ARGameObject currPlatform;
                for (int i = 0; i < len; i++)
                {
                    currPlatform = platforms[i];
                    showBigGrid(currPlatform.grid, cell);

                    // set the h values
                    setEuclidDistsOnSubGrid(currPlatform.grid, endPlatfrom.go.transform.position);

                    printCellDistances(currPlatform.grid);
                }

                // todo: move this call to appropriate place after done testing
                // todo: fix indexing to get correct grid on the start and end platforms
                bool foundPath = aStarSearch(getACellFromPlatform(startPlatform), getACellFromPlatform(endPlatfrom));
                logger.Log("foundPath? " + foundPath);
            }
            catch (Exception e)
            {
                logger.Log(e.ToString());
            }

            isGridifyDone = true;
        }
        else
        {
            Log3Text.text = "About to Delete plaform arcs";

            // delete the arcs as they are not needed anymore
            //try
            //{
            deleteAllPlatformArcClusters(platforms);
            /*}
            catch(Exception e)
            {
                Log3Text.text = "Exception: " + e.ToString();
            }*/

            Log3Text.text = "Deleted plaform arcs";
        }
    }

    // todo: this function is just until we figure out how to decide which start and end platform grid space to use
    private Cell getACellFromPlatform(ARGameObject aPlat)
    {
        List<List<Cell>> aGrid = aPlat.grid;
        for (int i = 0; i < aGrid.Count; i++)
        {
            List<Cell> aRow = aGrid[i];
            for (int j = 0; j < aRow.Count; j++)
            {
                Cell aCell = aRow[j];
                if (aCell != null)
                {
                    return aCell;
                }
            }
        }
        return null;
    }

    RaycastHit raycastHitWhich(Vector3 origin, Vector3 direction, float maxDist)
    {
        Ray aRay = jac.createRay(origin, direction);
        RaycastHit hits;
        int touchMask = LayerMask.GetMask("touchable");
        bool isRayHit = Physics.Raycast(aRay, out hits, maxDist, touchMask);

        return hits;
    }

    // this function takes a list of platforms and deletes their associated arcs
    public void deleteAllPlatformArcClusters(List<ARGameObject> listPlatforms)
    {
        int len = listPlatforms.Count;
        for (int i = 0; i < len; i++)
        {
            List<List<List<GameObject>>> currArcs = listPlatforms[i].arcs;
            jac.deleteListArcClusters(currArcs);
        }
    }

    List<List<Cell>> getBigGrid(GameObject platform, float stepSize)
    {
        List<List<Cell>> bigGrid = new List<List<Cell>>();
        float topLeftX = Mathf.Infinity;
        float topLeftZ = Mathf.Infinity;
        float topLeftY = Mathf.Infinity;
        float bottomRightX = Mathf.Infinity;
        float bottomRightZ = Mathf.Infinity;

        GameObject currPlat = null;
        Vector3 currMin = new Vector3();
        Vector3 currMax = new Vector3();
        Bounds currBounds = new Bounds();

        // get the largest z and smallest x coordinate (top left)
        // get smallest z and largest x coordinate (bottom right)
        currBounds = platform.GetComponent<Collider>().bounds;
        currMax = currBounds.max;
        currMin = currBounds.min;

        // top left check
        if (topLeftX == Mathf.Infinity || currMin.x < topLeftX)
        {
            topLeftX = currMin.x;
        }
        if (topLeftZ == Mathf.Infinity || currMax.z > topLeftZ)
        {
            topLeftZ = currMax.z;
        }
        if (topLeftY == Mathf.Infinity || currMax.y > topLeftY)
        {
            topLeftY = currMax.y;
        }

        // bottom right check
        if (bottomRightX == Mathf.Infinity || currMax.x > bottomRightX)
        {
            bottomRightX = currMax.x;
        }
        if (bottomRightZ == Mathf.Infinity || currMin.z < bottomRightZ)
        {
            bottomRightZ = currMin.z;
        }

        // todo: remove this. Only for debugging purposes.
        topLeftY = topLeftY + 5;

        // start at 1/2 step down and 1/2 step right of top left for first cell
        Vector3 topLeftVec = new Vector3(topLeftX, topLeftY, topLeftZ);
        Vector3 bottomRightVec = new Vector3(bottomRightX, topLeftY, bottomRightZ);
        Vector3 start = topLeftVec + new Vector3(stepSize / 2.0f, 0, -stepSize / 2.0f);

        int numRows = 0;
        int currNumCols = 0;

        Vector3 currCoord = start;
        // while x value less than rightmost, place cell there
        do
        {
            // reset number of cols
            currNumCols = 0;

            List<Cell> row = new List<Cell>();
            do
            {
                // create new cell
                Cell thisCell = new Cell(currCoord);

                // set its neighbors
                if (numRows > 0)
                {
                    /*logger.Log("setting above neighbor");
                    logger.Log("big grid so far: " + bigGrid[0].Count + "num cols: " + currNumCols);
                    */
                    // this is not the first row
                    // set the neighbor to the one above it in the previous row
                    Cell neighborCell = bigGrid[numRows - 1][currNumCols];
                    thisCell.upNeighborEdge = new Edge(neighborCell);
                    neighborCell.downNeighborEdge = new Edge(thisCell);
                    //logger.Log("Done setting above neighbor");
                }

                if (currNumCols > 0)
                {
                    //logger.Log("setting left neighbor");
                    int lastCellIdx = currNumCols - 1;
                    thisCell.leftNeighborEdge = new Edge(row[lastCellIdx]);
                    row[lastCellIdx].rightNeighborEdge = new Edge(thisCell);
                    //logger.Log("Done setting left neighbor");
                }

                // add cell to row
                row.Add(thisCell);
                currNumCols += 1;

                // increment by step size
                currCoord = currCoord + new Vector3(stepSize, 0, 0);
            } while (currCoord.x <= bottomRightVec.x);

            // done with that row. Moving to next row
            bigGrid.Add(row);
            numRows += 1;

            currCoord = new Vector3(start.x, start.y, currCoord.z - stepSize);
        } while (currCoord.z >= bottomRightVec.z);

        return bigGrid;
    }

    private void drawStraightLine(Vector3 pt1, Vector3 pt2, bool showPath = false)
    {
		GameObject lro = Instantiate(lineRendererObj);
		LineRenderer lr = lro.GetComponent<LineRenderer>();
        if (showPath)
        {
            logger.Log("trying to draw path line");
            lr.startColor = Color.black;
            lr.endColor = Color.black;
            lr.startWidth = 0.75f;
            lr.endWidth = 0.75f;
        }
        else
        {
            lr.startColor = Color.magenta;
            lr.endColor = Color.magenta;
            lr.startWidth = 0.5f;
            lr.endWidth = 0.5f;
        }
        

        Vector3[] positions = new Vector3[2];
        positions[0] = pt1;
        positions[1] = pt2;
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);
    }

    // pass the end node for aCell
    // todo: remove this when remove showBigGrid as it will fail if showBigGrid is not being called
    private void printPath(Cell aCell)
    {
        List<GameObject> nodes = new List<GameObject>();
        Cell currNode = aCell;
        nodes.Add(currNode.indicator);
        while (currNode.prev != null)
        {
            currNode = currNode.prev;
            nodes.Add(currNode.indicator);
        }

        int len = nodes.Count;
        GameObject prevGo = null;
        for (int i = len - 1; i >= 0; i--)
        {
            GameObject thisGameObj = nodes[i];
            thisGameObj.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            if(prevGo != null)
            {
                // draw line
                drawStraightLine(prevGo.transform.position, thisGameObj.transform.position, true);
            }
            prevGo = thisGameObj;
        }
        logger.Log("pathLen: " + aCell.g);
    }

    void showBigGrid(List<List<Cell>> bigGrid, GameObject indicator)
    {
        int numRows = bigGrid.Count;
        for (int i = 0; i < numRows; i++)
        {
            int numCols = bigGrid[i].Count;
            for (int j = 0; j < numCols; j++)
            {
                if (bigGrid[i][j] != null)
                {
                    GameObject aPoint = Instantiate(indicator);
                    aPoint.transform.position = bigGrid[i][j].position;
                    bigGrid[i][j].indicator = aPoint;

                    Cell thisCell = bigGrid[i][j];
                    logger.Log("(" + i + "," + j + "): " + thisCell.position);
                    if (thisCell.upNeighborEdge is null)
                    {
                        logger.Log("UpNeighborEdge: null");
                    }
                    else
                    {

                        drawStraightLine(thisCell.position, thisCell.upNeighborEdge.dest.position);
                        logger.Log("UpNeighbor: " + thisCell.upNeighborEdge.dest.position);
                    }

                    if (thisCell.downNeighborEdge is null)
                    {
                        logger.Log("DownNeighborEdge: null");
                    }
                    else
                    {
                        drawStraightLine(thisCell.position, thisCell.downNeighborEdge.dest.position);
                        logger.Log("DownNeighbor: " + thisCell.downNeighborEdge.dest.position);
                    }

                    if (thisCell.leftNeighborEdge is null)
                    {
                        logger.Log("LeftNeighborEdge: null");
                    }
                    else
                    {
                        drawStraightLine(thisCell.position, thisCell.leftNeighborEdge.dest.position);
                        logger.Log("LeftNeighbor: " + thisCell.leftNeighborEdge.dest.position);
                    }

                    if (thisCell.rightNeighborEdge is null)
                    {
                        logger.Log("RightNeighborEdge: null");
                    }
                    else
                    {
                        drawStraightLine(thisCell.position, thisCell.rightNeighborEdge.dest.position);
                        logger.Log("RightNeighbor: " + thisCell.rightNeighborEdge.dest.position);
                    }

                    // for the jump neighbors
                    List<Edge> jumpNeighs = thisCell.jumpReachableNeighborEdges;
                    int jumpNeighsLen = jumpNeighs.Count;
                    for (int k = 0; k < jumpNeighsLen; k++)
                    {
                        drawStraightLine(thisCell.position, jumpNeighs[k].dest.position);
                    }
                }
                else
                {
                    logger.Log("(" + i + "," + j + "): null");
                }
                logger.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }
        }

    }

    List<List<Cell>> gridifyPlatform(GameObject go, List<List<Cell>> grid)
    {
        int numRows = grid.Count;
        int numCols = grid[0].Count;

        Collider cldr = go.GetComponent<Collider>();

        // ray setup
        Ray ray = new Ray(new Vector3(), Vector3.down);
        RaycastHit hit;

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                Cell thisCell = grid[i][j];
                ray.origin = thisCell.position;
                if (cldr.Raycast(ray, out hit, Mathf.Infinity))
                {
                    thisCell.position = hit.point;
                    //GameObject aCell = Instantiate(cell);
                    //aCell.transform.position = thisCell.position;
                }
                else
                {
                    if (thisCell.upNeighborEdge != null)
                    {
                        (thisCell.upNeighborEdge.dest).downNeighborEdge = null;
                    }
                    if (thisCell.downNeighborEdge != null)
                    {
                        (thisCell.downNeighborEdge.dest).upNeighborEdge = null;
                    }
                    if (thisCell.leftNeighborEdge != null)
                    {
                        (thisCell.leftNeighborEdge.dest).rightNeighborEdge = null;
                    }
                    if (thisCell.rightNeighborEdge != null)
                    {
                        (thisCell.rightNeighborEdge.dest).leftNeighborEdge = null;
                    }
                    grid[i][j] = null;
                }
            }
        }

        return grid;
    }

    // note: the point should be the first point in a jump and the ARPlat should be the platform the jump starts from
    private Cell getClosestCell(ARGameObject ARPlat, GameObject point)
    {
        List<List<Cell>> thisGrid = ARPlat.grid;
        int numRows = thisGrid.Count;
        float smallestDist = Mathf.Infinity;
        Cell currShortestDistCell = null;
        float currDist;
        Cell currCell = null;
        for (int i = 0; i < numRows; i++)
        {
            List<Cell> thisRow = thisGrid[i];
            for (int j = 0; j < thisRow.Count; j++)
            {
                currCell = thisRow[j];
                if (currCell == null)
                {
                    continue;
                }
                currDist = Vector3.Distance(currCell.position, point.transform.position);
                if (currDist < smallestDist)
                {
                    smallestDist = currDist;
                    currShortestDistCell = currCell;
                }
            }
        }

        return currShortestDistCell;
    }

    private Cell getClosestCell(ARGameObject ARPlat, Vector3 point)
    {
        List<List<Cell>> thisGrid = ARPlat.grid;
        int numRows = thisGrid.Count;
        float smallestDist = Mathf.Infinity;
        Cell currShortestDistCell = null;
        float currDist;
        Cell currCell = null;
        for (int i = 0; i < numRows; i++)
        {
            List<Cell> thisRow = thisGrid[i];
            for (int j = 0; j < thisRow.Count; j++)
            {
                currCell = thisRow[j];
                if (currCell == null)
                {
                    continue;
                }
                currDist = Vector3.Distance(currCell.position, point);
                if (currDist < smallestDist)
                {
                    smallestDist = currDist;
                    currShortestDistCell = currCell;
                }
            }
        }

        logger.Log("<getClosestCell> returning: " + currShortestDistCell);
        return currShortestDistCell;
    }

    // use: bool intersectsPlatform(GameObject aPlatform, GameObject pt1, GameObject pt2)
    private void setJumpNeighbors(ARGameObject currPlat, Cell aCell, List<List<GameObject>> arcCluster)
    {
        int touchMask = LayerMask.GetMask("touchable");

        // for arc in the cluster
        int numArcs = arcCluster.Count;
        for (int i = 0; i < numArcs; i++)
        {
            logger.Log(i + "th arc");
            bool gotHighestPoint = false;
            Vector3 highestPoint = new Vector3();
            int numPoints = arcCluster[i].Count;
            for (int j = 1; j < numPoints; j++)
            {
                GameObject currPoint = arcCluster[i][j];
                GameObject lastPoint = arcCluster[i][j - 1];

                if(lastPoint.transform.position.y >= currPoint.transform.position.y)
                {
                    highestPoint = lastPoint.transform.position;
                    gotHighestPoint = true;
                }
                Vector3 dir = (currPoint.transform.position - lastPoint.transform.position).normalized;
                // see if intersects with another platform
                Ray aRay = new Ray(lastPoint.transform.position, dir);
                RaycastHit hit;
                // fix ray distance so it's NOT infinity!!!
                bool isRayHit = Physics.Raycast(aRay, out hit, Vector3.Distance(currPoint.transform.position, lastPoint.transform.position), touchMask);
                if (isRayHit)
                {
                    logger.Log("ray hit!!!");
                    GameObject groundGO = hit.collider.gameObject;
                    if (groundGO.CompareTag("ground") && groundGO != currPlat.go)
                    {
                        logger.Log("ground it hit");
                        // hit a ground/platform
                        if(gotHighestPoint)
                        {
                            // downward part of arc
                            // set the neighbor
                            ARGameObject thisARGO = getARGameObjectFromGameObject(groundGO);
                            logger.Log("thisARGO: " + thisARGO);
                            Cell neighborCell = getClosestCell(thisARGO, hit.point);
                            logger.Log("aCell: " + aCell);
                            logger.Log("neighborCell: " + neighborCell);

                            if (neighborCell != null)
                            {
                                aCell.jumpReachableNeighborEdges.Add(new Edge(neighborCell));
                                logger.Log("added aCell neighbors");
                                neighborCell.jumpReachableNeighborEdges.Add(new Edge(aCell));
                                logger.Log("Set neighbors: " + aCell.position + ", " + neighborCell.position);
                            }
                        }
                    }

                    // exit this loop, i.e. move onto the next arc in the arc since either we found a ground to jump onto or something was in the way
                    break;
                }
            }
        }
    }

    private ARGameObject getARGameObjectFromGameObject(GameObject platform)
    {
        int platformsLen = platforms.Count;
        for (int i = 0; i < platformsLen; i++)
        {
            // check if this platform is referring to the given GameObject
            if(platforms[i].go == platform)
            {
                return platforms[i];
            }
        }
        // unable to find the corresponding platform. This should never happen
        logger.Log("<getARGameObjectFromGameObject> returned null");
        return null;
    }

    private void setAllJumpNeighbors()
    {
        int platformsLen = platforms.Count;
        for (int i = 0; i < platformsLen; i++)
        {
            logger.Log("=== " + i + "th platform ===");
            ARGameObject currPlat = platforms[i];
            List<List<List<GameObject>>> clusters = currPlat.arcs;
            int clustersLen = clusters.Count;
            logger.Log("# clusters: " + clustersLen);

            for (int j = 0; j < clustersLen; j++)
            {
                logger.Log(j + "th cluster");
                // get first point in first arc
                List<List<GameObject>> currCluster = clusters[j];
                GameObject firstPoint = currCluster[0][0];
                logger.Log("firstPoint: " + firstPoint);
                logger.Log("firstPoint position: " + firstPoint.transform.position);
                // get the closest cell to this point
                Cell sourceCell = getClosestCell(currPlat, firstPoint);
                logger.Log("closest cell: " + sourceCell);

                if (sourceCell != null)
                {
                    setJumpNeighbors(currPlat, sourceCell, currCluster);
                }
            }
        }
    }
    /*
    bool intersectsPlatform(GameObject aPlatform, GameObject pt1, GameObject pt2)
    {
        Vector3 pos1 = pt1.transform.position;
        Vector3 pos2 = pt2.transform.position;
        Vector3 unitVec = (pos2 - pos1).normalized;

        RaycastHit rh = raycastHitWhich(pos1, unitVec);
        if (rh.collider == aPlatform.GetComponent<Collider>())
        {
            // the arc hit the given platform
            return true;
        }
        // did not hit
        return false;
    }

    RaycastHit raycastHitWhich(Vector3 origin, Vector3 direction)
    {
        Ray aRay = jac.createRay(origin, direction);
        RaycastHit hits;
        int touchMask = LayerMask.GetMask("touchable");
        bool isRayHit = Physics.Raycast(aRay, out hits, touchMask);

        return hits;
    }
    */

    // pass a grid of one of the platforms and the end cell position
    // will set the heuristic value to the Euclidian distance from that cell to the end cell position for each cell in that grid
    private void setEuclidDistsOnSubGrid(List<List<Cell>> grid, Vector3 endCellPos)
    {
        int numRows = grid.Count;
        List<Cell> currRow;
        Cell currCell;
        for (int i = 0; i < numRows; i++)
        {
            currRow = grid[i];
            for (int j = 0; j < currRow.Count; j++)
            {
                currCell = currRow[j];

                if (currCell != null)
                {
                    // get euclidian distance and set the heuristic value for that cell
                    currCell.h = Vector3.Distance(currCell.position, endCellPos);
                }
            }
        }
    }

    // for debugging purposes
    private void printCellDistances(List<List<Cell>> grid)
    {
        logger.Log("~~~ Printing cell h values ~~~");
        int numRows = grid.Count;
        List<Cell> currRow;
        Cell currCell;
        for (int i = 0; i < numRows; i++)
        {
            currRow = grid[i];
            for (int j = 0; j < currRow.Count; j++)
            {
                currCell = currRow[j];

                if (currCell != null)
                {
                    // get euclidian distance and set the heuristic value for that cell
                    logger.Log("cell " + currCell.position + ", h: " + currCell.h);
                }
            }
        }
        logger.Log("~~~~~~~");
    }

    private Cell getCellWithSmallestVal(List<Cell> cells)
    {
        int len = cells.Count;
        Cell minCell = null;
        float minVal = Mathf.Infinity;
        for (int i = 0; i < len; i++)
        {
            Cell currCell = cells[i];
            float currSum = currCell.g + currCell.h;
            if (currSum < minVal)
            {
                minVal = currSum;
                minCell = currCell;
            }
        }

        return minCell;
    }

    private Cell setNeighborVals(Cell cell, int dir, HashSet<Cell> cs)
    {
        Edge edge = null;
        switch(dir)
        {
            case 1:
                edge = cell.upNeighborEdge;
                break;
            case 2:
                edge = cell.downNeighborEdge;
                break;
            case 3:
                edge = cell.leftNeighborEdge;
                break;
            case 4:
                edge = cell.rightNeighborEdge;
                break;
            default:
                break;
        }

        if (edge == null)
        {
            return null;
        }
        Cell neighbor = edge.dest;

        if (!cs.Contains(neighbor))
        {
            float currLenToCell = cell.g + edge.weight;

            // if this is less than the current g + h value of that node, reset the neighbor's g value
            if (currLenToCell < neighbor.g)
            {
                neighbor.g = currLenToCell;
                neighbor.prev = cell;
            }
            return neighbor;
        }

        return null;
    }

    private void addPriority(Cell aCell, List<Cell> pq, HashSet<Cell> cs)
    {
        bool inPq = pq.Contains(aCell);
        bool inCs = cs.Contains(aCell);
        if (inPq)
            return;
        else if (aCell != null && !inPq && !inCs)
        {
            // if neighbor not in close set and not in priority queue, add to pq
            pq.Add(aCell);
        }
    }

    // A Star search
    private bool aStarSearch(Cell start, Cell end)
    {
        logger.Log("started aStar search. Start: " + start);
		start.g = 0;

		// priority queue data structure and close set
		List<Cell> pq = new List<Cell>(); // priority queue
        HashSet<Cell> cs = new HashSet<Cell>(); // close set
        // put the start cell in the priority queue
        pq.Add(start);

        Cell currCell = null;
        Cell neighCell = null;

        logger.Log("put start cell in the pq");
        while (pq.Count > 0)
        {
            logger.Log("pq count: " + pq.Count);
            // pop the highest priority cell, add to close set
            currCell = getCellWithSmallestVal(pq);
            cs.Add(currCell);
            pq.Remove(currCell);

            // if this is the goal, return
            if (currCell == end)
            {
                printPath(currCell);
                return true;
            }

            logger.Log("about to check up down left right neighbors.");
            // for each neighbor, add the g value of parent to the edge between parent and neighbor and add h
            // up
            neighCell = setNeighborVals(currCell, 1, cs);
            addPriority(neighCell, pq, cs);
            logger.Log("checked up neighbor");

            // down
            neighCell = setNeighborVals(currCell, 2, cs);
            addPriority(neighCell, pq, cs);
            logger.Log("checked down neighbor");

            // left
            neighCell = setNeighborVals(currCell, 3, cs);
            addPriority(neighCell, pq, cs);
            logger.Log("checked left neighbor");

            // right
            neighCell = setNeighborVals(currCell, 4, cs);
            addPriority(neighCell, pq, cs);
            logger.Log("checked right neighbor");

            // for jump locations
            int jumpCnt = currCell.jumpReachableNeighborEdges.Count;
            Edge edge = null;
            for (int i = 0; i < jumpCnt; i++)
            {
                edge = currCell.jumpReachableNeighborEdges[i];
                if (edge == null)
                {
                    continue;
                }
                Cell neighbor = edge.dest;
                if (!cs.Contains(neighbor))
                {
                    float currLenToCell = currCell.g + edge.weight;

                    // if this is less than the current g + h value of that node, reset the neighbor's g value
                    if (currLenToCell < neighbor.g)
                    {
                        neighbor.g = currLenToCell;
                        neighbor.prev = currCell;
                    }
                    addPriority(neighbor, pq, cs);
                }
            }
            logger.Log("completed checking jump neighbors");
        }
        return false;
    }
}
