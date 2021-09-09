using System;
using System.Threading.Tasks;

namespace masz.Events
{
    public delegate Task AsyncEventHandler<in TArgs>(TArgs e) where TArgs : EventArgs;
}