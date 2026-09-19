Public Class AppGroup
    Public ReadOnly Property Key As String
    Public ReadOnly Property Title As String
    Public ReadOnly Property Apps As List(Of AppConfig)

    Public Sub New(key As String, title As String, apps As List(Of AppConfig))
        Me.Key = key
        Me.Title = title
        Me.Apps = apps
    End Sub
End Class

Public Class AppGrouping
    Public Const UngroupedTitle As String = "(Ungrouped)"

    ''' <summary>Groups by FirmName; within a group orders by FinancialYear descending. This is the on-screen order.</summary>
    Public Shared Function GroupAndSort(apps As IEnumerable(Of AppConfig)) As List(Of AppGroup)
        Return apps.
            GroupBy(Function(x) If(String.IsNullOrWhiteSpace(x.FirmName), "", x.FirmName)).
            Select(Function(g) New AppGroup(
                g.Key,
                If(g.Key.Length = 0, UngroupedTitle, g.Key),
                g.OrderByDescending(Function(x) If(String.IsNullOrWhiteSpace(x.FinancialYear), "", x.FinancialYear)).ToList())).
            ToList()
    End Function
End Class
