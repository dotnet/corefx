using System;
using System.Collections.Generic;

namespace System
{
    public class AdditionalFunctions
    {
        //The two enums below these lines are referred to indices (i.e., positions of the elements in the input collection).
        //For example: the inputs {1, 2, 2} are assumed to be 3 different items, defined by their indices (i.e., 0, 1 and 2).
        //That's why a permutation including the positions 0-1-2 (i.e., values 1-2-2) is assumed to not have repetitions.
        public enum RepetitionsPermutation 
        { 
            NoRepetitions,
            AllRepetitions  
        };
        public enum OrderPermutation 
        { 
            AnyOrderSamePermutation, //1-2-3 is assumed to be identical to: 1-3-2, 2-3-1, 3-2-1 and 3-1-2.
            OneOrderOnePermutation   //All the aforementioned permutations are assumed to be different.
        };

        private static RepetitionsPermutation _Repetitions;
        private static OrderPermutation _Order;
        private static int mainI, maxVal, maxI;
        private static int[] indices; //Indices of all the items in the given permutation from the second position onwards.

        public static Dictionary<int, object[]> Permutations(object[] Items, int Size)
        {
            return Permutations(Items, Size, RepetitionsPermutation.NoRepetitions, OrderPermutation.AnyOrderSamePermutation);
        }

        public static Dictionary<int, object[]> Permutations(object[] Items, int Size, RepetitionsPermutation Repetitions)
        {
            return Permutations(Items, Size, Repetitions, OrderPermutation.AnyOrderSamePermutation);
        }

        public static Dictionary<int, object[]> Permutations(object[] Items, int Size, OrderPermutation Order)
        {
            return Permutations(Items, Size, RepetitionsPermutation.NoRepetitions, Order);
        }

        public static Dictionary<int, object[]> Permutations(object[] Items, int Size, RepetitionsPermutation Repetitions, OrderPermutation Order)
        {
            return getPermutations(Items, Size, Repetitions, Order);
        }

        private static Dictionary<int, object[]> getPermutations(object[] Items, int Size, RepetitionsPermutation Repetitions, OrderPermutation Order)
        {
            Dictionary<int, object[]> permutations = new Dictionary<int, object[]>();
         
            if (Size < 2 || Size > Items.Length)
            {
                for (int i0 = 0; i0 < Items.Length; i0++)
                {
                    permutations.Add(i0, new object[] { Items[i0] });
                }
            }

            _Repetitions = Repetitions;
            _Order = Order;

            maxVal = Items.Length - 1; //Last position in the input collection.
            maxI = Size - 2; //Last position in the indices array.

            for (int i = 0; i <= maxVal; i++) 
            {
                mainI = i;
                if (!resetIndices(-1)) break; //Reseting the values in the indices array.

                while (true)
                {
                    //Storing the values of the last permutation (i.e., mainI + indices).
                    permutations = AddPermutation(permutations, Items);

                    //Updating the values of the indices array.
                    if (!nextPermutations()) break; 
                }
            }

            return permutations;
        }

        private static Dictionary<int, object[]> AddPermutation(Dictionary<int, object[]> permutations, object[] Items)
        {
            object[] curPermutation = new object[maxI + 2];
            curPermutation[0] = Items[mainI];
            
            for (int i2 = 0; i2 < maxI + 1; i2++)
            {
                curPermutation[i2 + 1] = Items[indices[i2]];
            }
            permutations.Add(permutations.Count + 1, curPermutation); 

            return permutations;
        }

        private static bool nextPermutations()
        {
            int newValI = -1;
            int curI = maxI; //Position in the indices array whose value will be changed. By default, the last position.

            //This loop increases the value (indices[curI]) and decreases the position (curI) until finding a valid new value.
            while (newValI == -1 && curI >= 0) 
            {
                if (indices[curI] == maxVal)
                {
                    while (curI >= 0 && indices[curI] == maxVal)
                    {
                        curI = curI - 1; 
                    }
                }
                if (curI == -1) return false;

                newValI = newValue(indices[curI], curI); 
                indices[curI] = (newValI == -1 ? maxVal : newValI);
            }
            if (newValI == -1) return false; //No more valid permutations are possible for the current mainI value.

            //Reseting the values of all the positions after curI.
            resetIndices(curI + 1);

            return true;
        }

        static int newValue(int valI, int curI)
        {
            valI = valI + 1;

            //Special rules to the basic +1 approach for situations where no repetitions are allowed or when the order of the elements matters.
            if (_Repetitions == RepetitionsPermutation.NoRepetitions || _Order == OrderPermutation.AnyOrderSamePermutation)
            {
                while (valI <= maxVal && changeValue(valI, curI))
                {
                    valI = valI + 1;
                }
            }

            return (valI > maxVal ? -1 : valI);
        }

        static bool changeValue(int valI, int curI)
        {
            if (_Order == OrderPermutation.AnyOrderSamePermutation)
            {
                //The value at each position has to be greater (or equal, if repetitions are possible) than the one in the previous position.
                //For example: there will be only one permutation with 1-2-3, because all the other alternatives (i.e., 1-3-2, 2-1-3, 2-3-1, 3-2-1, 3-1-2) break that rule. 
                if (_Repetitions == RepetitionsPermutation.NoRepetitions && (valI > maxVal - (maxI - curI))) return true;
                if (valI < (curI == 0 ? mainI : indices[curI - 1])) return true;
            }

            if (_Repetitions == RepetitionsPermutation.NoRepetitions)
            {
                //Making sure that no other element of the permutation (i.e., mainI or any position in the indices array) has the same value than the current one.
                if (valI == mainI) return true;

                for (int i = 0; i < curI; i++) //The curI analysis is performed backwards and that's why all the positions after it are irrelevant.
                {
                    if (valI == indices[i]) return true;
                }
            }

            return false; //The new value is fine.
        }

        //This function has to be called after each variation in the index array to reset the values of the positions after it.
        static bool resetIndices(int startI) 
        {
            int val = -1;
            if (startI == -1)
            {
                startI = 0;
                indices = new int[maxI + 1];
            }

            bool isOK = true;
            for (int i = startI; i <= maxI; i++)
            {
                val = (_Repetitions == RepetitionsPermutation.AllRepetitions ? 0 : newValue(val, i)); 
                if (val == -1)
                {
                    indices[i] = maxVal;
                    isOK = false;
                }
                else
                {
                    if (_Order == OrderPermutation.AnyOrderSamePermutation)
                    {
                        int prevValI = (i == 0 ? mainI : indices[i - 1]);
                        if (val < prevValI) val = prevValI;
                    }
                    indices[i] = val;
                }
            }

            return isOK;
        }
    }
}
