using GraphVisual.GraphD;

namespace GraphVisual.Algorithm
{
    public interface IAlgorithm
    {
        CommunityStructure FindCommunityStructure(DGraph pGraph);
    }
}
