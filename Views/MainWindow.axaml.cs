using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace SvgEditorApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this._toolsPanel = EditToolsPanel;
    }
    
    private Control? _draggedShape;
    private Point _lastPointerPosition;
    private Rectangle? _selectionRectangle;
    private Control? _selectedShape;
    private readonly IBrush _selectionBrush = Brushes.Blue;
    private readonly IBrush _defaultStrokeBrush = Brushes.Black;
    private readonly StackPanel _toolsPanel;

    private void OnShapePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control shape)
        {
            // Выделяем фигуру
            SelectShape(shape);
            
            _draggedShape = shape;
            _lastPointerPosition = e.GetPosition(MainCanvas);
            e.Pointer.Capture(shape);
        }
    }

    private void SelectShape(Control shape)
    {
        // Удаляем предыдущий выделяющий прямоугольник
        if (_selectionRectangle != null)
        {
            MainCanvas.Children.Remove(_selectionRectangle);
            _selectionRectangle = null;
        }

        // Устанавливаем новый выделяющий прямоугольник
        _selectedShape = shape;

        if (_selectedShape != null)
        {
            Console.WriteLine(_selectedShape.Bounds.Height);
            Console.WriteLine(_selectedShape.Bounds.Width);
            var left = Canvas.GetLeft(_selectedShape);
            var top = Canvas.GetTop(_selectedShape);
            var selectionHeight = (_selectedShape.Bounds.Height > 0 ? _selectedShape.Bounds.Height : 50) + 10;
            var selectionWidth = (_selectedShape.Bounds.Width > 0 ? _selectedShape.Bounds.Width : 50) + 10;            _selectionRectangle = new Rectangle
            {
                Width = selectionWidth, 
                Height = selectionHeight, 
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                IsHitTestVisible = false // Чтобы прямоугольник не мешал взаимодействию с элементом
            };

            Canvas.SetLeft(_selectionRectangle, left - 5); 
            Canvas.SetTop(_selectionRectangle, top - 5);

            MainCanvas.Children.Add(_selectionRectangle);
        }

        _toolsPanel.IsVisible = true;
    }
    private void DeselectShape()
    {
        if (_selectionRectangle != null)
        {
            MainCanvas.Children.Remove(_selectionRectangle);
            _selectionRectangle = null;
        }

        _selectedShape = null;
        _toolsPanel.IsVisible = false;
    }
    
    // Изменение заливки фигуры
    private void OnFillColorChanged(object? sender, Avalonia.Controls.ColorChangedEventArgs e)
    {
        if (_selectedShape is Shape shape)
        {
            shape.Fill = new SolidColorBrush(e.NewColor);
        }
    }
    
    private void OnStrokeColorChanged(object? sender, Avalonia.Controls.ColorChangedEventArgs e)
    {
        if (_selectedShape is Shape shape)
        {
            shape.Stroke = new SolidColorBrush(e.NewColor);
        }
    }

    // Двигаем фигуру
    private void OnShapePointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedShape != null)
        {
            var currentPosition = e.GetPosition(MainCanvas);
            var offsetX = currentPosition.X - _lastPointerPosition.X;
            var offsetY = currentPosition.Y - _lastPointerPosition.Y;

            var left = Canvas.GetLeft(_draggedShape) + offsetX;
            var top = Canvas.GetTop(_draggedShape) + offsetY;

            Canvas.SetLeft(_draggedShape, left);
            Canvas.SetTop(_draggedShape, top);

            // Update the selection rectangle position
            if (_selectionRectangle != null)
            {
                Canvas.SetLeft(_selectionRectangle, left - 5); // Adjust for the margin
                Canvas.SetTop(_selectionRectangle, top - 5);
            }

            _lastPointerPosition = currentPosition;
        }
    }

    // Отпускаем фигуру
    private void OnShapePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggedShape != null)
        {
            e.Pointer.Capture(null);
            _draggedShape = null;
        }
    }
    
    private void OnRectangleClick(object? sender, PointerPressedEventArgs e)
    {
        var rect = new Rectangle
        {
            Width = 50,
            Height = 50,
            Fill = Brushes.Transparent,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        AddShapeToCanvas(rect);
        SelectShape(rect);
    }
    
    private void OnEllipseClick(object? sender, PointerPressedEventArgs e)
    {
        var ellipse = new Ellipse
        {
            Width = 50,
            Height = 50,
            Fill = Brushes.Transparent,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        AddShapeToCanvas(ellipse);
        SelectShape(ellipse);
    }
    
    private void OnTriangleClick(object? sender, PointerPressedEventArgs e)
    {
        var triangle = new Polygon
        {
            Points = new Points
            {
                new Point(0, 50),
                new Point(25, 0),
                new Point(50, 50)
            },
            Fill = Brushes.Transparent,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        AddShapeToCanvas(triangle);
        SelectShape(triangle);
    }

    private void OnCanvasPress(object? sender, PointerPressedEventArgs e)
    {
        Console.WriteLine("Canvas Clicked");
        // Снимаем выделение с фигуры, если кликнули на пустое место
        if (e.Source == MainCanvas)
        {
            DeselectShape();
        }
    }
    
    private void AddShapeToCanvas(Control shape)
    {
        Canvas.SetLeft(shape, 10);
        Canvas.SetTop(shape, 10);
        shape.PointerPressed += OnShapePointerPressed;
        shape.PointerMoved += OnShapePointerMoved;
        shape.PointerReleased += OnShapePointerReleased;
        
        MainCanvas.Children.Add(shape);
    }

    // Методы для изменения свойств выделенной фигуры
    private void OnChangeColorClick(object? sender, PointerPressedEventArgs e)
    {
        if (_selectedShape is Shape shape)
        {
            shape.Fill = Brushes.Red; // Можно заменить на выбор цвета из диалога
        }
    }
    
    private void OnChangeSizeClick(object? sender, PointerPressedEventArgs e)
    {
        if (_selectedShape != null)
        {
            _selectedShape.Width = 100; // Новый размер
            _selectedShape.Height = 100;
            
            // Для треугольника нужно обновить Points
            if (_selectedShape is Polygon polygon)
            {
                polygon.Points = new Points
                {
                    new Point(0, 100),
                    new Point(50, 0),
                    new Point(100, 100)
                };
            }
            // Для линии нужно обновить StartPoint и EndPoint
            else if (_selectedShape is Line line)
            {
                line.StartPoint = new Point(0, 0);
                line.EndPoint = new Point(100, 100);
            }
        }
    }
    
    private void OnDeleteShapeClick(object? sender, PointerPressedEventArgs e)
    {
        if (_selectedShape != null)
        {
            MainCanvas.Children.Remove(_selectedShape);
            _selectedShape = null;
        }
    }
}