namespace RPG.Utils
{
    public interface IPredicateEvaluator
    {
        public enum PredicateType
        {
            HasItemEquipped,
            HasInventoryItem,
            HasQuest,
            CompletedQuest,
            MinimumTrait,
            IsAtPlace,
            HasInventoryItems,
            CompletedObjectiveOfQuest,
            HasActionItemEquipped
        }
        bool? Evaluate(PredicateType predicate, string[] parameters);
    }

}