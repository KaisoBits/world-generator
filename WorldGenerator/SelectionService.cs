namespace WorldGenerator;

public class SelectionService
{
    private readonly World _world;

    public SelectionService(World world)
    {
        _world = world;
    }

    public ITileView? SelectedTile { get; private set; }

    public void SelectTile(int x, int y)
    {
        SelectedTile = _world[x, y];
    }

    public void UnselectTile()
    {
        SelectedTile = null;
    }

    public void SelectTile(Vector position) => SelectTile(position.X, position.Y);
}
