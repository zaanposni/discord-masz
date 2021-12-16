namespace MASZ.Events
{
    public delegate Task AsyncEventHandler<in TArgs>(TArgs e) where TArgs : EventArgs;
}