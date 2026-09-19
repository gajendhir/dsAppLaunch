''' <summary>
''' Which apps are ticked for "Launch Selected" on the main screen. Session-only: seeded from
''' AppConfig.Selected on each (re)load and never written back to apps.json. Keyed by object
''' reference so hand-edited configs with duplicate Ids still behave.
''' </summary>
Public Class SelectionState
    Private ReadOnly selected As New HashSet(Of AppConfig)()

    Public ReadOnly Property Count As Integer
        Get
            Return selected.Count
        End Get
    End Property

    Public Sub ResetFrom(apps As IEnumerable(Of AppConfig))
        selected.Clear()
        For Each app As AppConfig In apps
            If app.Selected Then selected.Add(app)
        Next
    End Sub

    Public Function IsSelected(app As AppConfig) As Boolean
        Return selected.Contains(app)
    End Function

    Public Sub SetSelected(app As AppConfig, value As Boolean)
        If value Then selected.Add(app) Else selected.Remove(app)
    End Sub

    Public Sub SetAll(apps As IEnumerable(Of AppConfig), value As Boolean)
        For Each app As AppConfig In apps
            SetSelected(app, value)
        Next
    End Sub

    ''' <summary>The selected apps, in the order of <paramref name="ordered"/> (the on-screen order).</summary>
    Public Function SelectedIn(ordered As IEnumerable(Of AppConfig)) As List(Of AppConfig)
        Return ordered.Where(Function(a) selected.Contains(a)).ToList()
    End Function
End Class
