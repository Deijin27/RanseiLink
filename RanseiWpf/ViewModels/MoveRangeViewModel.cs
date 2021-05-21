using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RanseiWpf.ViewModels
{
    public class MoveRangeViewModel : ViewModelBase, IViewModelForModel<IMoveRange>
    {
        public IMoveRange Model { get; set; }

        #region Row0

        public bool Row0Col0
        {
            get => Model.GetInRange(3 * 8 + 5);
            set => Model.SetInRange(3 * 8 + 5, value);
        }

        public bool Row0Col1
        {
            get => Model.GetInRange(3 * 8 + 1);
            set => Model.SetInRange(3 * 8 + 1, value);
        }

        public bool Row0Col2
        {
            get => Model.GetInRange(3 * 8 + 2);
            set => Model.SetInRange(3 * 8 + 2, value);
        }

        #endregion

        #region Row1

        public bool Row1Col0
        {
            get => Model.GetInRange(3 * 8 + 0);
            set => Model.SetInRange(3 * 8 + 0, value);
        }

        public bool Row1Col1
        {
            get => Model.GetInRange(1 * 8 + 1);
            set => Model.SetInRange(1 * 8 + 1, value);
        }

        public bool Row1Col2
        {
            get => Model.GetInRange(1 * 8 + 2);
            set => Model.SetInRange(1 * 8 + 2, value);
        }

        #endregion

        #region Row2

        public bool Row2Col0
        {
            get => Model.GetInRange(1 * 8 + 0);
            set => Model.SetInRange(1 * 8 + 0, value);
        }

        public bool Row2Col1
        {
            get => Model.GetInRange(0 * 8 + 1);
            set => Model.SetInRange(0 * 8 + 1, value);
        }

        public bool Row2Col2
        {
            get => Model.GetInRange(0 * 8 + 2);
            set => Model.SetInRange(0 * 8 + 2, value);
        }

        #endregion

        #region Row3

        public bool Row3Col0
        {
            get => Model.GetInRange(0 * 8 + 7);
            set => Model.SetInRange(0 * 8 + 7, value);
        }

        public bool Row3Col1
        {
            get => Model.GetInRange(0 * 8 + 0);
            set => Model.SetInRange(0 * 8 + 0, value);
        }

        public bool Row3Col2
        {
            get => Model.GetInRange(0 * 8 + 3);
            set => Model.SetInRange(0 * 8 + 3, value);
        }

        #endregion

        #region Row4

        public bool Row4Col0
        {
            get => Model.GetInRange(0 * 8 + 6);
            set => Model.SetInRange(0 * 8 + 6, value);
        }

        public bool Row4Col1
        {
            get => Model.GetInRange(0 * 8 + 5);
            set => Model.SetInRange(0 * 8 + 5, value);
        }

        public bool Row4Col2
        {
            get => Model.GetInRange(0 * 8 + 4);
            set => Model.SetInRange(0 * 8 + 4, value);
        }

        #endregion

        #region Row5

        public bool Row5Col0
        {
            get => Model.GetInRange(2 * 8 + 2);
            set => Model.SetInRange(2 * 8 + 2, value);
        }

        public bool Row5Col1
        {
            get => Model.GetInRange(2 * 8 + 1);
            set => Model.SetInRange(2 * 8 + 1, value);
        }

        public bool Row5Col2
        {
            get => Model.GetInRange(2 * 8 + 0);
            set => Model.SetInRange(2 * 8 + 0, value);
        }

        #endregion

    }
}
