using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalculatorApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Clears Input Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            //
            this.textBox1.Clear();
            this.FocusInputText();
        }

        /// <summary>
        /// Focus the userinput text
        /// </summary>

        private void FocusInputText()
        {
            this.textBox1.Focus();
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("0");
            FocusInputText();
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            this.DeleteTextValue();
            FocusInputText();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("7");
            FocusInputText();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("8");
            FocusInputText();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("9");
            FocusInputText();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("4");
            FocusInputText();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("5");
            FocusInputText();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("6");
            FocusInputText();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("1");
            FocusInputText();
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("2");
            FocusInputText();
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("3");
            FocusInputText();
        }

        private void InsertTextValue(string value)
        {
            //remember Selection Start
            var selectionStart = this.textBox1.SelectionStart;

            //Set new text
            this.textBox1.AppendText(value);

            //Restore the selection start
            this.textBox1.SelectionStart = selectionStart + 1;

            //Set selection length to zero
            this.textBox1.SelectionLength = 0;
        }

        private void DeleteTextValue()
        {
            if (textBox1.Text.Length < this.textBox1.SelectionStart + 1)
            {
                return;
            }

            //remember Selection Start
            var selectionStart = this.textBox1.SelectionStart;

            //Set new text
            this.textBox1.Text = this.textBox1.Text.Remove(this.textBox1.SelectionStart, 1);

            //Restore the selection start
            this.textBox1.SelectionStart = selectionStart;

            //Set selection length to zero
            this.textBox1.SelectionLength = 0;
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("/");
            FocusInputText();
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("X");
            FocusInputText();
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("+");
            FocusInputText();
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            this.InsertTextValue("-");
            FocusInputText();
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            this.InsertTextValue(".");
            FocusInputText();
        }

        private void CalculateEquation()
        {
            var userInput = this.textBox1.Text;

            var result = ParseOPeration();
            Output.Text = result;

            var MyEnum =(int) OperationType.Multiply;
        }

        private string ParseOPeration()
        {
            try
            {
                // Get the users equation input
                var input = this.textBox1.Text;

                // Remove all spaces
                input = input.Replace(" ", "");

                // Create a new top-level operation
                var operation = new Operation();
                var leftSide = true;

                // Loop through each character of the input
                // starting from the left working to the right
                for (int i = 0; i < input.Length; i++)
                {
                    // TODO: Handle order priority
                    //       4.2 + 5.7 * 3
                    //       It should calculate 5 * 3 first, then 4 + the result (so 4 + 15)

                    // Check if the current character is a number
                    if ("0123456789.".Any(c => input[i] == c))
                    {
                        if (leftSide)
                            operation.LeftSide = AddNumberPart(operation.LeftSide, input[i]);
                        else
                            operation.RightSide = AddNumberPart(operation.RightSide, input[i]);
                    }
                    // If it is an operator ( + - * / ) set the operator type
                    else if ("+-X/".Any(c => input[i] == c))
                    {
                        // If we are on the right side already, we now need to calculate our current operation
                        // and set the result to the left side of the next operation
                        if (!leftSide)
                        {
                            // Get the operator type
                            var operatorType = GetOperationType(input[i]);

                            // Check if we actually have a right side number
                            if (operation.RightSide.Length == 0)
                            {
                                // Check the operator is not a minus (as they could be creating a negative number)
                                if (operatorType != OperationType.Minus)
                                    throw new InvalidOperationException($"Operator (+ * / or more than one -) specified without an right side number");

                                // If we got here, the operator type is a minus, and there is no left number currently, so add the minus to the number
                                operation.RightSide += input[i];
                            }
                            else
                            {
                                // Calculate previous equation and set to the left side
                                operation.LeftSide = CalculateOperation(operation);

                                // Set new operator
                                operation.OperationType = operatorType;

                                // Clear the previous right number
                                operation.RightSide = string.Empty;
                            }
                        }
                        else
                        {
                            // Get the operator type
                            var operatorType = GetOperationType(input[i]);

                            // Check if we actually have a left side number
                            if (operation.LeftSide.Length == 0)
                            {
                                // Check the operator is not a minus (as they could be creating a negative number)
                                if (operatorType != OperationType.Minus)
                                    throw new InvalidOperationException($"Operator (+ * / or more than one -) specified without an left side number");

                                // If we got here, the operator type is a minus, and there is no left number currently, so add the minus to the number
                                operation.LeftSide += input[i];
                            }
                            else
                            {
                                // If we get here, we have a left number and now an operator, so we want to move to the right side

                                // Set the operation type
                                operation.OperationType = operatorType;

                                // Move to the right side
                                leftSide = false;
                            }
                        }
                    }
                }

                // If we are done parsing, and there were no exceptions
                // calculate the current operation
                return CalculateOperation(operation);
            }
            catch (Exception ex)
            {
                return $"Invalid equation. {ex.Message}";
            }
        }

        private string CalculateOperation(Operation operation)
        {
            double left = 0;
            double right = 0;

            // Check if we have a valid left side number
            if (string.IsNullOrEmpty(operation.LeftSide) || !double.TryParse(operation.LeftSide, out left))
                throw new InvalidOperationException($"Left side of the operation was not a number. {operation.LeftSide}");

            // Check if we have a valid right side number
            if (string.IsNullOrEmpty(operation.RightSide) || !double.TryParse(operation.RightSide, out right))
                throw new InvalidOperationException($"Right side of the operation was not a number. {operation.RightSide}");

            try
            {
                switch (operation.OperationType)
                {
                    case OperationType.Add:
                        return (left + right).ToString();
                    case OperationType.Minus:
                        return (left - right).ToString();
                    case OperationType.Divide:
                        return (left / right).ToString();
                    case OperationType.Multiply:
                        return (left * right).ToString();
                    default:
                        throw new InvalidOperationException($"Unknown operator type when calculating operation. { operation.OperationType }");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to calculate operation {operation.LeftSide} {operation.OperationType} {operation.RightSide}. {ex.Message}");
            }
        }

        private OperationType GetOperationType(char CharacterType)
        {
            switch (CharacterType)
            {
                case '+':
                    return OperationType.Add;
                case '-':
                    return OperationType.Minus;
                case '/':
                    return OperationType.Divide;
                case 'X':
                    return OperationType.Multiply;
                default:
                    throw new InvalidOperationException($"Unknown operator type { CharacterType }");
            }
        }

        private string AddNumberPart(string currentNumber, char newCharacter)
        {
            // Check if there is already a . in the number
            if (newCharacter == '.' && currentNumber.Contains('.'))
                throw new InvalidOperationException($"Number {currentNumber} already contains a . and another cannot be added");

            return currentNumber + newCharacter;

        }

        private void Button18_Click(object sender, EventArgs e)
        {
            CalculateEquation();
        }
    }
}
