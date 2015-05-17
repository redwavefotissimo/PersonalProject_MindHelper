using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MindHelper.Math
{
    class Expression
    {
        private string _Expression;
        private delegate int _SearchIndexMethod(string Expression);
        private Dictionary<string, string> _MathConstants;
        private List<string> _functionList,
                             _OperationList;
        private MindHelper.Reflection.ClassReflector _ClassReflector;

        #region LOAD

        /// <summary>
        /// Class Constructor:
        /// </summary>
        public Expression()
        {
            Initialize();
        }

        /// <summary>
        /// Class Constructor:
        /// </summary>
        /// <param name="Expression">Expression To be Processed.</param>
        public Expression(string Expression)
        {
            _Expression = StringFormatter.RemoveWhiteSpace(Expression);
            Initialize();
        }

        /// <summary>
        /// Loads an Expression to be Processed.
        /// </summary>
        /// <param name="Expression">Expression To be Process.</param>
        public void LoadExpression(string Expression)
        {
            _Expression = StringFormatter.RemoveWhiteSpace(Expression);
        }

        /// <summary>
        /// Initializes required settings; 
        /// </summary>
        public void Initialize()
        {
            _MathConstants = new Dictionary<string, string>();
            _OperationList = new List<string>();
            _functionList = new List<string>();
            _ClassReflector = new Reflection.ClassReflector();
            // Loads the Operation
            _ClassReflector.LoadClass(typeof(OperationConstants));
            _OperationList.AddRange(_ClassReflector.GetValueFieldConstantList().Keys.ToArray<string>());
            _MathConstants = CollectionUtility.MergeDictionary<string, string>(_MathConstants, _ClassReflector.GetValueFieldConstantList());
            // Loads the Function
            _ClassReflector.LoadClass(typeof(FunctionConstants));
            _functionList.AddRange(_ClassReflector.GetValueFieldConstantList().Keys.ToArray<string>());
            _MathConstants = CollectionUtility.MergeDictionary<string, string>(_MathConstants, _ClassReflector.GetValueFieldConstantList());
            // loads the class that holds the methods to be invoked
            _ClassReflector.LoadClass(typeof(Solve));
        }

        #endregion

        #region PROCESS

        #region MAIN

        /// <summary>
        /// Gets the answer by processing the expression given.
        /// </summary>
        /// <returns>Answer for the Expression.</returns>
        public string Solve()
        {
            return Solve(_Expression);
        }
        
        /// <summary>
        /// Recursive: Split by Grouping, Function, Operation then Solve the Expression until there is nothing to be Split.
        /// </summary>
        /// <param name="Expression">Main Expression, to be Split then solve.</param>
        /// <param name="LeftExpression">Left Expression to be merged with the main expression after split then solve.</param>
        /// <param name="RightExpression">Right Expression to be merged with the main expression after split then solve.</param>
        /// <returns>Answer for the Expression.</returns>
        private string Solve(string Expression = "", string LeftExpression = "", string RightExpression = "")
        {
            if (CollectionUtility.CollectionItemContainsInValue(Expression, GroupingConstants.OpenGroupingList))
            {
                string[] Expressions = GroupSplitter(Expression);
                Expression = Solve(Expressions[1], Expressions[0], Expressions[2]);
            }
            else if (CollectionUtility.CollectionItemContainsInValue(Expression, _functionList.ToArray()))
            {
                Expression = GetEquivalentOfSimilarExpression(Expression, GetFirstIndexOfAvailableFunction);
            }
            else if (CollectionUtility.CollectionItemContainsInValue(Expression, _OperationList.ToArray()))
            {
                Expression = GetEquivalentOfSimilarExpression(Expression, GetIndexOfHighestAvailabeOperation);
            }

            // merge the Left, Main and Right Expression to be solve again until there is nothing to be solve.
            Expression = LeftExpression + Expression + RightExpression;

            double DummyHolder = 0;
            if (double.TryParse(Expression, out DummyHolder))
            {
                return Expression;
            }
            else
            {
                return Solve(Expression);
            }
        }

        /// <summary>
        /// Recursive: Split by Function/Operation then Solve the Expression until there is nothing to be Split.
        /// </summary>
        /// <param name="Expression">Expression to be Split then solve.</param>
        /// <param name="GetindexMethod">Invokes/run either the Operation index getter or Function index getter Method.</param>
        /// <returns>Answer for the Expression</returns>
        private string GetEquivalentOfSimilarExpression(string Expression, _SearchIndexMethod GetindexMethod)
        {
            string[] Expressions = Splitter(Expression, GetindexMethod);

            Expressions[1] = GetEquivalent(Expressions[1]);

            // merge the Left, Main and Right Expression to be solve again until there is nothing to be solve.
            Expression = Expressions[0] + Expressions[1] + Expressions[2];

            double DummyHolder = 0;
            if (double.TryParse(Expression, out DummyHolder))
            {
                return Expression;
            }
            else // at this point the expression is not yet convertable to a digit only expression.
            {
                if ((int) GetindexMethod(Expression) < 0) // The current index searcher is now invalid and must return the expression to use another index searcher.
                {
                    return Expression;
                }
                else // if the index searcher returns non negative value it means it can still go on.
                {
                    return GetEquivalentOfSimilarExpression(Expression, GetindexMethod);
                }
            }
        }

        /// <summary>
        /// Splits into 3 item, the Left and Right Expression to be merged later and the Main Expression to be Solved.
        /// </summary>
        /// <param name="Expression">Expression to be Split.</param>
        /// <param name="GetIndexMethod">The Method to be used in getting the starting index to be Split.</param>
        /// <returns>The Splitted Expression (Left, Main, Right).</returns>
        private string[] Splitter(string Expression, _SearchIndexMethod GetIndexMethod)
        {
            string[] Expressions = new string[] { string.Empty, string.Empty, string.Empty };
            int SearchedIndex = (int) GetIndexMethod(Expression),
                SelectedIndex = 1;

            // no more function or operation to be search/split, 
            // just return the expression in hopes that it can be converted into a number,
            // otherwise a stackoverflow exception may occur here.
            if (SearchedIndex == -1) 
            {
                Expressions[SelectedIndex] = Expression;
            }
            else
            {
                Expressions[SelectedIndex] = Expression[SearchedIndex].ToString();
                // Get Left Expression
                for (int i = SearchedIndex - 1; i >= 0; i--)
                {
                    if (IsOperationOrFunction(Expression, i))
                    {
                        SelectedIndex = 0; // place to first item.
                    }
                    Expressions[SelectedIndex] = Expression[i] + Expressions[SelectedIndex];
                }
                SelectedIndex = 1;
                // Get Right Expression
                for (int i = SearchedIndex + 1; i < Expression.Length; i++)
                {
                    if (IsOperationOrFunction(Expression, i))
                    {
                        SelectedIndex = 2; // place to third item.
                    }
                    Expressions[SelectedIndex] += Expression[i];
                }
            }

            return Expressions;
        }

        /// <summary>
        /// Solve the only expression, no left and right expression to be split.
        /// </summary>
        /// <param name="Expression">Expression to be solve.</param>
        /// <returns>Answer for the Expression.</returns>
        private string GetEquivalent(string Expression)
        {
            List<object> Values = new List<object>();
            string MethodName = string.Empty,
                   DummyHolder = string.Empty;
            for (int i = 0; i < Expression.Length; i++)
            {
                // also watch for negative numbers (number is automatically treated as negative number if previous expression is also non number non decimal point)
                // or if the first character is a non number non decimal non functional
                if ((!_functionList.Contains(Expression[i].ToString()) && !_OperationList.Contains(Expression[i].ToString())) || Expression[i].ToString() == "." || // include all other character in the dummy holder.
                    (i == 0 && !_functionList.Contains(Expression[i].ToString())) || // the first index should not be a function to be included in the dummy holder.
                    (i - 1 > 0 && _OperationList.Contains(Expression[i - 1].ToString()))) // the previous index should be an operation for the current non number non decimal to be included in the dummy holder.
                {
                    DummyHolder += Expression[i].ToString();
                }
                else
                {
                    // only try convert to number if not empty or whitespace
                    if (!string.IsNullOrWhiteSpace(DummyHolder))
                    {
                        Values.Add((double)Converter.ConvertTo(DummyHolder, Converter.Types.Double));
                    }
                    DummyHolder = string.Empty;
                    MethodName = _MathConstants[Expression[i].ToString()];
                }
            }

            if (!string.IsNullOrEmpty(DummyHolder))
            {
                Values.Add((double)Converter.ConvertTo(DummyHolder, Converter.Types.Double));
            }

            return (string)_ClassReflector.MethodInvoker(MethodName, Converter.Types.String, Values.ToArray());
        }

        /// <summary>
        /// Checks if the current character is a function/operation or not.
        /// </summary>
        /// <param name="Expression">The Expression to be checked.</param>
        /// <param name="Index">The character position in the Expression to be checked.</param>
        /// <returns></returns>
        private bool IsOperationOrFunction(string Expression, int Index)
        {
            if (_OperationList.Contains(Expression[Index].ToString()) || _functionList.Contains(Expression[Index].ToString()))
            {
                // also watch for negative numbers (number is automatically treated as negative number if previous expression is also non number non decimal point)
                if (Index - 1 >= 0 && !_OperationList.Contains(Expression[Index - 1].ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region GROUPING

        /// <summary>
        /// The Second Item always contain the Expression with the first open group found. While the First and Third Item Always contain the rest of the expression to be merged later.
        /// </summary>
        /// <param name="Expression">Expression to be splitted.</param>
        /// <returns>Returns Splitted Expression, The Collection always contains 3 items.</returns>
        private string[] GroupSplitter(string Expression)
        {
            int OpenGroupCount = 0,
                SelectedIndex = 0;
            string[] Expressions = new string[] { string.Empty, string.Empty, string.Empty };

            foreach (Char Character in Expression)
            {
                if (!ContainsInGroupingList(Character.ToString()) || // includes all Non Grouping Character
                   (SelectedIndex == 1 && PreviewOpenGroupCount(OpenGroupCount, Character) > 0) || // Removes the First Grouping (Outer most)
                   (SelectedIndex == 2)) // includes all Character (also includes grouping) at this point.
                {
                    Expressions[SelectedIndex] += Character.ToString();
                }
                if (GroupingConstants.OpenGroupingList.Contains(Character.ToString()) && SelectedIndex != 2)
                {
                    if (OpenGroupCount == 0)
                        SelectedIndex = 1; // move to the Second Item.
                    OpenGroupCount++;
                }
                if (GroupingConstants.CloseGroupingList.Contains(Character.ToString()) && SelectedIndex != 2)
                {
                    OpenGroupCount--;
                    if (OpenGroupCount == 0)
                        SelectedIndex = 2; // move to the Third Item.
                }
            }

            return Expressions;
        }

        /// <summary>
        /// Pre Process to decrement OpenGrouoCount if the character belongs to Close grouping.
        /// </summary>
        /// <param name="OpenGroupCount">OpenGroupCount to be changed.</param>
        /// <param name="Character">Character to evaluate.</param>
        /// <returns>Returns pre processed OpenGroupCount Value.</returns>
        private int PreviewOpenGroupCount(int OpenGroupCount, Char Character)
        {
            if (GroupingConstants.CloseGroupingList.Contains(Character.ToString()))
                OpenGroupCount--;
            return OpenGroupCount;
        }

        /// <summary>
        /// Checks if the character is in the open or close grouping list.
        /// </summary>
        /// <param name="Character">Character to be checked.</param>
        /// <returns>Returns True if in the groupinglist otherwise False.</returns>
        private bool ContainsInGroupingList(string Character)
        {
            return GroupingConstants.OpenGroupingList.Contains(Character) || GroupingConstants.CloseGroupingList.Contains(Character);
        }

        #endregion

        #region FUNCTION

        /// <summary>
        /// Gets the First index of Available Function.
        /// </summary>
        /// <param name="Expression">Expression to be searched.</param>
        /// <returns>The searched index.</returns>
        private int GetFirstIndexOfAvailableFunction(string Expression)
        {
            int ReturningIndex = -1;

            for (int i = 0; i < Expression.Length; i++)
            {
                if (_functionList.Contains(Expression[i].ToString()))
                {
                    ReturningIndex = i;
                    break;
                }
            }

            return ReturningIndex;
        }

        #endregion

        #region OPERATION

        /// <summary>
        /// Gets the first index of highest Available Operation.
        /// </summary>
        /// <param name="Expression">Expression to be searched.</param>
        /// <returns>The searched index.</returns>
        private int GetIndexOfHighestAvailabeOperation(string Expression)
        {
            int ReturningIndex = -1;

            foreach (string Operation in _OperationList)
            {
                ReturningIndex = Expression.IndexOf(Operation);
                if (ReturningIndex > -1)
                {
                    // if first operational index is at 0 it means the number is a negative number (-), therefore the next operational index is to be considered
                    if (ReturningIndex == 0)
                    {
                        ReturningIndex = Expression.IndexOf(Operation, 1);
                    }
                    break;
                }
            }

            return ReturningIndex;
        }

        #endregion

        #endregion
    }
}
