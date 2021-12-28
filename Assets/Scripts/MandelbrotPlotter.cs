using Mandelbrot.Collections;
using Mandelbrot.Jobs;
using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot {

  [Serializable]
  public struct Settings {
    [Tooltip("Number of max iteratiosn per point")]
    public int Iterations;
    [Tooltip("Viewport where the mandelbrot set will be calculated")]
    public Viewport Viewport;
    [Tooltip("Color of the points inside the mandelbrot set")]
    public Color MandelbrotColor;
    [Range(0, 1)]
    public float H;
    [Range(0, 1)]
    public float S;
    [Range(0, 1)]
    public float V;

    public float HPower;
    public float SPower;
    public float VPower;

    public float3 HSV {
      get => new float3(H, S, V);
    }

    public float3 HSVPower {
      get => new float3(HPower, SPower, VPower);
    }

    public override int GetHashCode() {
      unchecked {
        var hash = 17;
        hash *= 23 + Iterations.GetHashCode();
        hash *= 23 + Viewport.GetHashCode();
        hash *= 23 + MandelbrotColor.GetHashCode();
        hash *= 23 + HSV.GetHashCode();
        hash *= 23 + HSVPower.GetHashCode();
        return hash;
      }
    }
  }

  public class MandelbrotPlotter : MonoBehaviour {
    [SerializeField]
    bool _dotsMode;
    [SerializeField]
    int _width = 1024;
    [SerializeField]
    int _height = 1024;
    [SerializeField]
    FilterMode _filterMode;
    [SerializeField]
    Settings _settings = new Settings {
      Iterations = 255,
      Viewport = new Viewport { Min = new float2(-2), Max = new float2(2) },
      MandelbrotColor = Color.green,
      H = 0.25f,
      S = 1,
      V = 0.9f
    };
    [SerializeField]
    float _zoomFactor = 0.5f;
    [SerializeField]
    TMPro.TMP_Text _textLabel;

    Texture2D _texture;
    int _settingsHash = 0;

    bool _plotting;
    bool _zooming;
    float _zoomTime;
    Viewport _originalViewport;
    Viewport _startViewport;
    Viewport _targetViewport;
    [SerializeField]
    float _zoomDuration = 2f;
    [SerializeField]
    AnimationCurve _zoomCurve;

    void Start() {
      _settingsHash = _settings.GetHashCode();
      _texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
      _originalViewport = _settings.Viewport;
    }

    void Update() {
      if (Input.GetKeyDown(KeyCode.R))
        _settings.Viewport = _originalViewport;

      if (_plotting && Input.GetMouseButtonDown(0))
        SetZoom(true,
          Utilities.GetScreenPointInsideViewport(Camera.main, this, _settings.Viewport, (Vector2)Input.mousePosition), _zoomFactor);

      if (_plotting && Input.GetMouseButtonDown(1))
        SetZoom(false, Utilities.GetScreenPointInsideViewport(Camera.main, this, _settings.Viewport, (Vector2)Input.mousePosition), _zoomFactor);

      _settingsHash = Input.anyKey ? 0 : _settingsHash;

      if (_settings.GetHashCode() != _settingsHash) {
        _plotting = true;
        _settingsHash = _settings.GetHashCode();
        var w = new Stopwatch();
        w.Start();
        var totalIterations = GenerateColors(_width, _height);
        w.Stop();

        var stats =
          $"Executed {totalIterations} iterations in {w.ElapsedMilliseconds}ms.\n" +
          $"Average of {(float)totalIterations / (_width * _height)} iterations per point\n" +
          $"Resolution of {_width}x{_height} with {_width * _height} points and range {_settings.Viewport}";
        _textLabel?.SetText(stats);
        print(stats);
      }

      if (_settingsHash != 0 && _zooming)
        Zoom(_startViewport, _targetViewport);
    }

    int GenerateColors(int width, int height) {
      using var totalIterations = new NativeCounter(Allocator.TempJob);
      using var colors = new NativeArray<Color32>(width * height, Allocator.TempJob);
      var jobHandle = new GenerateColorBurstJob {
        Step = new double2(_settings.Viewport.Width / width, _settings.Viewport.Height / height),
        Width = width,
        Height = height,
        Iterations = _settings.Iterations,
        Viewport = _settings.Viewport,
        HSVBase = _settings.HSV,
        HSVPower = _settings.HSVPower,
        DefaultColor = _settings.MandelbrotColor,
        Colors = colors,
        TotalIterations = totalIterations
      }.ScheduleParallel(_width, 128, default);
      jobHandle.Complete();

      _texture.LoadRawTextureData(colors);
      _texture.filterMode = _filterMode;
      _texture.Apply();
      GetComponent<Renderer>().sharedMaterial.mainTexture = _texture;
      return totalIterations.Value;
    }

    void Zoom(Viewport source, Viewport target) {
      _zoomTime += Time.deltaTime / _zoomDuration;
      _zooming = _zoomTime < 1;
      var min = math.lerp(source.Min, target.Min, _zoomCurve.Evaluate(_zoomTime));
      var max = math.lerp(source.Max, target.Max, _zoomCurve.Evaluate(_zoomTime));
      _settings.Viewport = new Viewport { Min = min, Max = max };
    }

    void SetZoom(bool _in, Viewport target, float zoomFactor) {
      var factor = _in ? zoomFactor : 1f / zoomFactor;
      _startViewport = _settings.Viewport;
      _targetViewport = target;
      _targetViewport.Width = _startViewport.Width * factor;
      _targetViewport.Height = _startViewport.Height * factor;
      _zooming = true;
      _zoomTime = 0;
    }
  }
}
