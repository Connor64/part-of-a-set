public class KeyVal<TKey, TVal> {
    public TKey Key;
    public TVal Val;

    public KeyVal() { }

    public KeyVal(TKey key, TVal val) {
        Key = key;
        Val = val;
    }
}