using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Linq.Tests
{
    public class SkipWhileTests
    {
        [Fact]
        public void Fundamentals()
        {
            bool[] bools = { true, true, true, false, false, true };
            
            IEnumerable<bool> skipped = bools.SkipWhile(b => b);
            
            bool[] expected = { false, false, true };
            bool[] actual = skipped.ToArray();
            
            Assert.True(expected.SequenceEqual(actual));
        }
        
        [Fact]
        public void ReturnEmptyAndDoNothingForEmptyEnumerables()
        {
            int[] ints = new int[0];
            
            IEnumerable<int> results = ints.SkipWhile(i =>
            {
                Assert.True(false, "Predicate should not be called since collection has no elements.");
                return false;
            });
            
            Assert.Equal(results.Count(), 0);
        }
        
        [Fact]
        public void ReturnEquivalentEnumerableOnFirstItemReturningFalse()
        {
            int[] ints = { 1, 2, 3 };
            
            IEnumerable<int> results = ints.SkipWhile(i => false);
            
            Assert.True(ints.SequenceEqual(results));
        }
        
        [Fact]
        public void ReturnNothingIfEverythingIsSkipped()
        {
            string[] array = { "quick", "brown", "ghfox" };
            
            IEnumerable<string> results = array.SkipWhile(b => true);
            
            Assert.Equal(results.Count(), 0);
        }
        
        [Fact]
        public void EnsureCorrectIndexIsPassedIn()
        {
            int[] array = { 1, 2, 3, 5, 8 };
            
            IEnumerable<int> results = array.SkipWhile(
                (element, index) =>
            {
                Assert.Equal(element, array[index]);
                return true;
            });
        }
    }
}
