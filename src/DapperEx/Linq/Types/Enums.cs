namespace DapperEx.Linq.Types
{
    public enum LikeMethod
    {
        StartsWith,
        EndsWith,
        Contains,
        Equals
    }

   
    public enum AverageFunction
    {
        COUNT,
        DISTINCT,
        SUM,
        MIN,
        MAX,
        AVG
    }

    public enum JoinType
    {
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin
    }
}
