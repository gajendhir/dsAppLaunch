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

    ''' <summary>Groups by Group; within a group orders by SortOrder ascending, then SubGroup descending. This is the on-screen order.</summary>
    Public Shared Function GroupAndSort(apps As IEnumerable(Of AppConfig)) As List(Of AppGroup)
        Return apps.
            GroupBy(Function(x) If(String.IsNullOrWhiteSpace(x.Group), "", x.Group)).
            Select(Function(g) New AppGroup(
                g.Key,
                If(g.Key.Length = 0, UngroupedTitle, g.Key),
                g.OrderBy(Function(x) x.SortOrder).ThenByDescending(Function(x) If(String.IsNullOrWhiteSpace(x.SubGroup), "", x.SubGroup)).ToList())).
            ToList()
    End Function

    ''' <summary>All apps in on-screen order (group by group, card by card).</summary>
    Public Shared Function ScreenOrder(groups As IEnumerable(Of AppGroup)) As List(Of AppConfig)
        Return groups.SelectMany(Function(g) g.Apps).ToList()
    End Function
End Class
