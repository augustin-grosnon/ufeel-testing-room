public class AlwaysPassCondition : FogCondition
{
    public override bool CanPass()
    {
        return true;
    }
}
