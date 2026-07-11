public class AlwaysNotPassCondition : FogCondition
{
    public override bool CanPass()
    {
        return false;
    }
}
