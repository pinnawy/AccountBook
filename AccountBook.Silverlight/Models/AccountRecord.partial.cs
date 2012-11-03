using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;

namespace AccountBook.Model
{
    public partial class AccountRecord
    {
        private OperationBase _currentOperation;
        [Display(AutoGenerateField = false)]
        public OperationBase CurrentOperation
        {
            get
            {
                return this._currentOperation;
            }
            set
            {
                if (this._currentOperation != value)
                {
                    if (this._currentOperation != null)
                    {
                        this._currentOperation.Completed -= CurrentOperationChanged;
                    }

                    this._currentOperation = value;

                    if (this._currentOperation != null)
                    {
                        this._currentOperation.Completed += CurrentOperationChanged;
                    }

                    this.CurrentOperationChanged();
                }
            }
        }

        private void CurrentOperationChanged(object sender, EventArgs e)
        {
            this.CurrentOperationChanged();
        }

        /// <summary>
        /// Gets a value indicating whether the user is presently being registered or logged in.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public bool IsOperationing
        {
            get
            {
                return this.CurrentOperation != null && !this.CurrentOperation.IsComplete;
            }
        }

        /// <summary>
        /// Helper method for when the current operation changes.
        /// Used to raise appropriate property change notifications.
        /// </summary>
        private void CurrentOperationChanged()
        {
            this.RaisePropertyChanged("IsOperationing");
        }
    }
}
