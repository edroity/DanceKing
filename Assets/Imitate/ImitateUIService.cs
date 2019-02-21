public static class ImitateUIService
{
    public static void Dialog(string text)
    {
        var floating = UIEngine.ShowFloating<ImitateDialogFloating>();
        floating.PlayFadeOut(text);
    }
}