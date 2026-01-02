using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TpLab.SceneResolver.Editor
{
    /// <summary>
    /// Resolve属性が付いたフィールドの解決状況を表示・管理するエディタウィンドウ
    /// </summary>
    public sealed class SceneResolveWindow : EditorWindow
    {
        // 列名定数
        const string ColumnStatus = "status";
        const string ColumnGameObject = "gameObject";
        const string ColumnComponent = "component";
        const string ColumnField = "field";
        const string ColumnSource = "source";
        const string ColumnMessage = "message";

        [SerializeField]
        VisualTreeAsset visualTreeAsset;

        ResolveAnalyzer _analyzer;
        MultiColumnListView _listView;
        Label _summaryLabel;
        ToolbarMenu _statusFilter;
        ResolveStatus? _currentFilter;

        // フィルタリング済みのレポートをキャッシュ
        List<ResolveReport> _filteredReports;

        /// <summary>
        /// Scene Resolveウィンドウを開く
        /// </summary>
        [MenuItem("Tools/Scene Resolve")]
        public static void OpenWindow()
        {
            var window = GetWindow<SceneResolveWindow>();
            window.titleContent = new GUIContent("Scene Resolve");
        }

        /// <summary>
        /// UIを初期化する
        /// </summary>
        public void CreateGUI()
        {
            var root = rootVisualElement;
            root.Add(visualTreeAsset.Instantiate());

            _analyzer = new ResolveAnalyzer();
            _summaryLabel = root.Q<Label>("summary-label");
            _listView = root.Q<MultiColumnListView>("resolve-list");
            _statusFilter = root.Q<ToolbarMenu>("status-filter");

            var refreshButton = root.Q<ToolbarButton>();
            refreshButton.clicked += Refresh;

            ConfigureStatusFilter();
            ConfigureColumns();
            ConfigureSorting();
            Refresh();
        }

        /// <summary>
        /// ウィンドウが破棄される際にイベントハンドラーを解除
        /// </summary>
        void OnDestroy()
        {
            if (_listView != null)
            {
                _listView.columnSortingChanged -= OnColumnSortingChanged;
            }
        }

        /// <summary>
        /// ステータスフィルタードロップダウンメニューを設定
        /// </summary>
        void ConfigureStatusFilter()
        {
            _statusFilter.menu.AppendAction("All", _ => SetFilter(null), _ =>
                _currentFilter == null ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            _statusFilter.menu.AppendAction("Success", _ => SetFilter(ResolveStatus.Success), _ =>
                _currentFilter == ResolveStatus.Success ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            _statusFilter.menu.AppendAction("Error", _ => SetFilter(ResolveStatus.Error), _ =>
                _currentFilter == ResolveStatus.Error ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
        }

        /// <summary>
        /// フィルターを設定し、リストビューを更新
        /// </summary>
        /// <param name="status">フィルターするステータス。nullの場合は全て表示</param>
        void SetFilter(ResolveStatus? status)
        {
            _currentFilter = status;
            _statusFilter.text = status?.ToString() ?? "All";
            ApplyFilter();
        }

        /// <summary>
        /// 現在のフィルター設定に基づいてリストを更新し、サマリーを表示
        /// </summary>
        void ApplyFilter()
        {
            _filteredReports = _currentFilter == null
                ? _analyzer.Reports
                : _analyzer.Reports.Where(r => r.Status == _currentFilter.Value).ToList();

            _listView.itemsSource = _filteredReports;
            _listView.Rebuild();

            UpdateSummary();
        }

        /// <summary>
        /// サマリーラベルを更新（成功/エラー件数を表示）
        /// </summary>
        void UpdateSummary()
        {
            var successCount = _analyzer.Reports.Count(r => r.Status == ResolveStatus.Success);
            var errorCount = _analyzer.Reports.Count(r => r.Status == ResolveStatus.Error);

            if (_currentFilter == null)
            {
                _summaryLabel.text = $"Total: {_analyzer.Reports.Count} | ✓ {successCount} | ✗ {errorCount}";
            }
            else
            {
                _summaryLabel.text = $"Showing: {_filteredReports.Count} / {_analyzer.Reports.Count} | ✓ {successCount} | ✗ {errorCount}";
            }
        }

        /// <summary>
        /// 各列のバインディングを設定
        /// </summary>
        void ConfigureColumns()
        {
            SetupStatusColumn();
            SetupGameObjectColumn();
            SetupColumn(ColumnComponent, r => r.ComponentName);
            SetupColumn(ColumnField, r => r.FieldName);
            SetupColumn(ColumnSource, r => r.Source.ToString());
            SetupColumn(ColumnMessage, r => r.Message);
        }

        /// <summary>
        /// 全列のソート機能を有効化し、ソートイベントを設定
        /// </summary>
        void ConfigureSorting()
        {
            foreach (var column in _listView.columns)
            {
                column.sortable = true;
            }

            _listView.columnSortingChanged += OnColumnSortingChanged;
        }

        /// <summary>
        /// 列ヘッダークリック時のソート処理
        /// </summary>
        void OnColumnSortingChanged()
        {
            var sortedColumn = _listView.sortedColumns.FirstOrDefault();
            if (sortedColumn == null) return;

            var itemsSource = _listView.itemsSource as List<ResolveReport>;
            if (itemsSource == null) return;

            var ascending = sortedColumn.direction == SortDirection.Ascending;
            var sortedList = SortReports(itemsSource, sortedColumn.columnName, ascending);

            _listView.itemsSource = sortedList;
            _listView.Rebuild();
        }

        /// <summary>
        /// レポートリストを指定した列でソート
        /// </summary>
        /// <param name="reports">ソート対象のレポートリスト</param>
        /// <param name="columnName">ソート基準となる列名</param>
        /// <param name="ascending">昇順の場合true、降順の場合false</param>
        /// <returns>ソート済みのレポートリスト</returns>
        List<ResolveReport> SortReports(List<ResolveReport> reports, string columnName, bool ascending)
        {
            Func<ResolveReport, object> keySelector = columnName switch
            {
                ColumnStatus => r => r.Status,
                ColumnGameObject => r => r.GameObject?.name ?? string.Empty,
                ColumnComponent => r => r.ComponentName ?? string.Empty,
                ColumnField => r => r.FieldName ?? string.Empty,
                ColumnSource => r => r.Source,
                ColumnMessage => r => r.Message ?? string.Empty,
                _ => r => r.Status
            };

            return ascending
                ? reports.OrderBy(keySelector).ToList()
                : reports.OrderByDescending(keySelector).ToList();
        }

        /// <summary>
        /// 一般的な列のセルバインディングを設定
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="getter">レポートから表示テキストを取得する関数</param>
        void SetupColumn(string columnName, Func<ResolveReport, string> getter)
        {
            var column = _listView.columns[columnName];
            column.bindCell = (element, index) =>
            {
                var report = GetReportAtIndex(index);
                if (report != null)
                {
                    var label = element.Q<Label>();
                    label.text = getter(report);
                }
            };
        }

        /// <summary>
        /// Status列のセルバインディングを設定（バッジスタイル付き）
        /// </summary>
        void SetupStatusColumn()
        {
            var column = _listView.columns[ColumnStatus];
            column.bindCell = (element, index) =>
            {
                var report = GetReportAtIndex(index);
                if (report != null)
                {
                    var label = element.Q<Label>();
                    label.text = report.Status.ToString();
                    ApplyStatusBadgeStyle(label, report.Status);
                }
            };
        }

        /// <summary>
        /// GameObject列のセルバインディングを設定（クリック可能）
        /// </summary>
        void SetupGameObjectColumn()
        {
            var column = _listView.columns[ColumnGameObject];
            column.bindCell = (element, index) =>
            {
                var report = GetReportAtIndex(index);
                if (report != null)
                {
                    var label = element.Q<Label>();
                    label.text = report.GameObject.name;
                    MakeClickableCell(label, report);
                }
            };
        }

        /// <summary>
        /// 指定されたインデックスのレポートを取得
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>レポート、または存在しない場合はnull</returns>
        ResolveReport GetReportAtIndex(int index)
        {
            return _listView.itemsSource is List<ResolveReport> itemsSource && index < itemsSource.Count ? itemsSource[index] : null;
        }

        /// <summary>
        /// セルをクリック可能にし、クリック時にGameObjectを選択する
        /// </summary>
        /// <param name="label">対象のラベル</param>
        /// <param name="report">関連するレポート</param>
        void MakeClickableCell(Label label, ResolveReport report)
        {
            label.style.cursor = StyleKeyword.Auto;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;

            // ラベルにクリックイベントを登録（bindCellは再利用されるため毎回設定が必要）
            label.RegisterCallback<MouseDownEvent>(_ =>
            {
                if (report?.GameObject != null)
                {
                    Selection.activeGameObject = report.GameObject;
                    EditorGUIUtility.PingObject(report.GameObject);
                }
            });
        }

        /// <summary>
        /// ステータスに応じたバッジスタイルを適用
        /// </summary>
        /// <param name="label">対象のラベル</param>
        /// <param name="status">ステータス</param>
        void ApplyStatusBadgeStyle(Label label, ResolveStatus status)
        {
            label.ClearClassList();
            label.AddToClassList("status-badge");

            switch (status)
            {
                case ResolveStatus.Success:
                    label.AddToClassList("status-badge-success");
                    break;
                case ResolveStatus.Error:
                    label.AddToClassList("status-badge-error");
                    break;
            }
        }

        /// <summary>
        /// シーンを再分析し、リストを更新
        /// </summary>
        void Refresh()
        {
            _analyzer.Analyze();
            ApplyFilter();
        }
    }
}