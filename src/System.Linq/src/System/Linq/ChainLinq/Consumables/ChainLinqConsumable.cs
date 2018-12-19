namespace System.Linq.ChainLinq.Consumables
{
    /// <summary>
    /// To indentify internal use of Consumable, if was ever to break out of the boundaries
    /// of System.Linq.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract class InternalChainLinqConsumable<T> : Consumable<T>
    {
    }
}
