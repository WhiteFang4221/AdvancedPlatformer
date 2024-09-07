public interface IVampirable
{
    public bool IsVampireTarget { get;}

    public void ReactToVampirism(float damage);

    public void GetOutVampirism();
}