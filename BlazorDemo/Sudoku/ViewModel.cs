using BlazorDemo.Utils;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Sudoku
{
    public class ViewModel : ObservableObject<ViewModel>
    {
        private Board board = Board.New();

        private bool isValid = true;
        public bool IsValid
        {
            get => isValid;
            set
            {
                SetAndNotify(ref isValid, value);
                SolveCommand.RaiseCanExecuteChanged();
            }
        }

        private int? selectedIndex;
        public int? SelectedIndex
        {
            get => selectedIndex;
            set => SetAndNotify(ref selectedIndex, value);
        }

        public DelegateCommand SolveCommand { get; }
        public DelegateCommand ClearCommand { get; }

        public event Action<int> TileChanged;

        public ViewModel()
        {
            SolveCommand = new DelegateCommand(Solve, () => isValid);
            ClearCommand = new DelegateCommand(Clear);
        }

        public int GetNumber(int index) => board[index];

        public void SetNumber(int index, int number)
        {
            board[index] = number;
            Validate();
            TileChanged?.Invoke(index);
        }

        public void TileClick(int index)
        {
            SelectedIndex = index;

            int number = (board[index] + 1) % 10;
            SetNumber(index, number);
        }

        public void TileKeyDown(KeyboardEventArgs e)
        {
            void OffsetSelectedIndex(int offset)
            {
                int oldIndex = selectedIndex ?? -1;
                SelectedIndex = (oldIndex + offset + Board.Size) % Board.Size;
            }

            void TrySetNumber(int number)
            {
                if (selectedIndex is int index)
                {
                    SetNumber(index, number);
                }
            }

            switch (e.Key)
            {
                case "ArrowUp":
                case "w":
                    OffsetSelectedIndex(-9);
                    return;
                case "ArrowRight":
                case "d":
                    OffsetSelectedIndex(1);
                    return;
                case "ArrowDown":
                case "s":
                    OffsetSelectedIndex(9);
                    return;
                case "ArrowLeft":
                case "a":
                    OffsetSelectedIndex(-1);
                    return;
                case "Escape":
                    SelectedIndex = null;
                    return;
                case "Backspace":
                case "Delete":
                    TrySetNumber(0);
                    return;
                case "c":
                    Clear();
                    return;
            }

            if (int.TryParse(e.Key, out int value))
            {
                value = Math.Clamp(value, 0, 9);
                TrySetNumber(value);
            }
        }

        public void Clear()
        {
            board.Clear();
            IsValid = true;
            TileChanged?.Invoke(-1);
        }

        public void Validate()
        {
            IsValid = board.Validate();
        }

        public void Solve()
        {
            if (board.TrySolve())
            {
                TileChanged?.Invoke(-1);
            }
        }
    }
}
