namespace Template.MobileApp.Usecase;

// アプリの CognitiveUsecase.cs にある検出結果 (矩形は 0〜1 に正規化した座標)
public record DetectResult(
    float Left,
    float Top,
    float Right,
    float Bottom,
    float Score,
    string Label);
