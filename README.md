# Unity 2048

Windows ve Android için klasik 2048 projesi. Sprint 1.1 tamamlandı; statik ve
responsive 4x4 oyun ekranı hazırdır. Sprint 1.2 TileView kurulumu da Unity
Editor'da doğrulandı. Oyun algoritması, input, dinamik skor ve New Game
davranışı henüz eklenmemiştir.

Unity sürümü: **6000.6.2f1** (`ProjectSettings/ProjectVersion.txt`).
Mevcut proje URP 2D şablonunu kullanır.

## Proje yapısı

| Klasör | Amaç |
|---|---|
| `Assets/Scenes` | Sahneler; mevcut `SampleScene.unity` korunmuştur |
| `Assets/Scripts/Core` | Gelecek sprintlerde oyun mantığı |
| `Assets/Scripts/UI` | Gelecek sprintlerde görünüm kodu |
| `Assets/Scripts/Input` | Gelecek sprintlerde kontrol kodu |
| `Assets/Prefabs` | Tekrar kullanılabilir nesneler |
| `Assets/Sprites` | 2D görseller |
| `Assets/Materials` | Materyaller |
| `Assets/Audio` | Ses varlıkları |
| `Assets/Settings`, `Assets/Welcome` | Mevcut Unity şablonu varlıkları |
| `Packages` | Mevcut paket tanımları |
| `ProjectSettings` | Unity proje ayarları |

Boş klasörler `.gitkeep` ile Git'te korunur. Unity `.meta` dosyalarını
ilgili varlıklarla birlikte sürüm kontrolünde tutun.
`Library`, `Temp`, `Logs`, `obj`, `Build`, `Builds` ve `UserSettings`
üretilen yerel verilerdir; `.gitignore` ile dışlanır.

## Unity Editor'da açma ve Sprint 0.1 testi

1. Unity Hub üzerinden `6000.6.2f1` sürümünü kullanarak bu kök klasörü
   (`Assets`, `Packages`, `ProjectSettings` içeren klasör) açın.
2. Paket çözümlemesi, import ve script derlemesi tamamlanana kadar bekleyin.
3. `Assets/Scenes/SampleScene.unity` sahnesini açın.
4. `Window > General > Console` içinde compiler error olmadığını doğrulayın.
5. Project penceresinde yukarıdaki temel klasörlerin bulunduğunu doğrulayın.
   Bu sprintte Inspector referansı bağlama işlemi yoktur.
6. Proje kökünde aşağıdaki komutları çalıştırın. İlk komut her yol için bir
   ignore kuralı göstermeli; ikinci komut çıktı vermemelidir:

```powershell
git check-ignore -v Library/ Temp/ Logs/ obj/ Build/ Builds/ UserSettings/
git ls-files -- Library Temp Logs obj Build Builds UserSettings
```

Kabul kriterleri kullanıcı tarafından Unity Editor'da test edilip onaylanana
kadar Sprint 0.1 `[-]` olarak kalır. Otomatik dosya kontrolleri Editor onayı
yerine geçmez. Onay sonrasında yalnızca sprint değişiklikleri seçilerek
`chore: initialize Unity 2048 project` mesajıyla commit oluşturulmalıdır;
önceden var olan kullanıcı değişiklikleri bu commit'e karıştırılmamalıdır.

## Sprint 1.1: statik oyun ekranı kurulumu

Sprint 1.1 kapsamındaki statik ekran düzeni Unity Editor'da kuruldu ve görsel
olarak doğrulandı. `New Game` butonunda henüz oyun davranışı yoktur; oyun
mantığı, kutu değeri, hareket, skor güncelleme ve input sonraki sprintlerin
kapsamındadır.

`Assets/Scenes/SampleScene.unity` sahnesini açın ve aşağıdaki hiyerarşiyi
oluşturun. Metin nesneleri için `UI > Text - TextMeshPro` kullanın. İlk TMP
nesnesini oluştururken Unity'nin önerdiği TMP Essential Resources import
işlemini tamamlayın.

```text
Canvas
├── Background
├── SafeContent
│   ├── Header
│   │   ├── Title
│   │   ├── ScorePanel
│   │   │   ├── Label
│   │   │   └── Value
│   │   ├── HighScorePanel
│   │   │   ├── Label
│   │   │   └── Value
│   │   └── NewGameButton
│   │       └── Text (TMP)
│   └── Board
│       ├── Cell_00
│       ├── Cell_01
│       ├── Cell_02
│       ├── Cell_03
│       ├── Cell_10
│       ├── Cell_11
│       ├── Cell_12
│       ├── Cell_13
│       ├── Cell_20
│       ├── Cell_21
│       ├── Cell_22
│       ├── Cell_23
│       ├── Cell_30
│       ├── Cell_31
│       ├── Cell_32
│       └── Cell_33
└── EventSystem
```

1. `Canvas` için `Render Mode: Screen Space - Overlay` seçin. `Canvas Scaler`
   bileşeninde `Scale With Screen Size`, `Reference Resolution: 1080 x 1920`
   ve `Screen Match Mode: Match Width Or Height` seçin. `Match` değeri
   `0` (`Width`) olarak ayarlanmıştır. Ekran portrait tutulur.
2. `Background` nesnesini tam ekrana stretch anchor ile yerleştirin ve bir
   `Image` bileşeni ekleyin. Açık, düz bir arka plan rengi kullanın.
3. `SafeContent` nesnesini yatayda `80`, üstte `120`, altta `96` piksel
   boşluk bırakacak şekilde stretch anchor ile yerleştirin. Bu değerler
   sonraki responsive UI sprintinde Safe Area davranışı eklendiğinde
   güncellenecektir.
4. `Header` nesnesini `SafeContent` içinde üstte stretch anchor ile
   yerleştirin ve yüksekliğini `220` yapın. `Title` metnini `2048`, font
   boyutunu `96`, hizalamasını sol-orta yapın. `ScorePanel` ve
   `HighScorePanel` nesnelerine `Image` ekleyin; içlerindeki label metinleri
   sırasıyla `SKOR` ve `EN İYİ`, değer metinleri ise başlangıçta `0` olur.
   Score ve Best alanlarında rounded-corner görünümü kullanılır; değerler
   bu sprintte statiktir.
5. `NewGameButton` için `UI > Button - TextMeshPro` oluşturun, metnini
   `YENİ OYUN` yapın ve Header'ın sağ tarafına yerleştirin. Butonda
   rounded-corner görünümü kullanılır ve `On Click` listesi boş bırakılır.
6. `Board` nesnesine `Image` ve `Grid Layout Group` ekleyin. Board'u
   `SafeContent` içinde Header'ın altına, yatayda stretch anchor ile
   yerleştirin. Genişlik ve yüksekliği eşit tutun; `Grid Layout Group`
   ayarları `Constraint: Fixed Column Count`, `Constraint Count: 4`,
   `Spacing: 16 x 16`, `Padding: 16` olmalıdır. `Child Alignment` için
   `Middle Center` seçin. Board arka planında rounded-corner görünümü
   kullanılır.
7. `Cell_00` için `UI > Image` oluşturun, istenen hücre rengini verin ve
   `Board` altına taşıyın. Ardından 15 kez çoğaltıp yukarıdaki isimlerle
   adlandırın. Grid Layout Group, her birini dört satır ve dört sütun olacak
   şekilde eşit boyutlu yerleştirir. Hücrelerde rounded-corner görünümü
   kullanılır; hücrelere henüz metin veya script eklenmez.
8. Game görünümünde `1080 x 1920`, `720 x 1280` ve `1080 x 2400` veya
   `720 x 1600` çözünürlüklerini seçerek başlık, skor panelleri, buton ve
   4x4 tahtanın çakışmadığını denetleyin. Sahneyi kaydedin.

4x4 Board, Score, Best ve New Game alanları bu kurulumda tamamlanmıştır.

## Sprint 1.2: TileView kurulumu

`Assets/Scripts/UI/TileView.cs`, hücrenin arka plan rengi ve TMP değer metnini
yönetir. Oyun kuralı, input veya sayı üretimi içermez.

1. Mevcut 16 `Cell_*` nesnesinin her birinin altında `Value` adlı bir
   `UI > Text - TextMeshPro` nesnesi oluşturun. `Value` RectTransform'unu
   hücreyi tamamen kaplayacak şekilde stretch anchor ile ayarlayın; metni
   ortalayın ve başlangıç metnini boş bırakın. Mevcut hücre konumlarını,
   renklerini, boyutlarını ve `ImageWithRoundedCorners` bileşenlerini
   değiştirmeyin.
2. Her `Cell_*` nesnesine `TileView` bileşenini ekleyin. `Background` alanına
   aynı Cell nesnesinin normal `UnityEngine.UI.Image` bileşenini atayın;
   `ImageWithRoundedCorners` bileşenini hücre üzerinde koruyun, ancak
   `TileView` Inspector alanına atamayın. `Value Text` alanına alt nesnedeki
   `Value` TMP bileşenini atayın.
3. Hücreleri Play Mode'da sınamak için bileşenin bağlam menüsündeki
   `Preview` seçeneklerini kullanın. `Empty (0)` metni gizlemeli; `2`, `4`,
   `8` ve `16` seçenekleri farklı arka plan renkleri göstermelidir. `2048`
   ve `4096` preview seçenekleri de açık metin rengi ve dört basamaklı font
   boyutuyla doğru görüntülenmelidir.
4. Herhangi bir Inspector referansı boşsa Play Mode'da Console, ilgili hücre
   adıyla birlikte eksik `Background` veya `Value Text` referansını bildirir.
   Referansı atayıp testi yeniden çalıştırın.

Kapsam ve kabul kriterleri: [2048_PROJECT_PLAN.md](2048_PROJECT_PLAN.md).

## Sprint 2.1: BoardModel EditMode testleri

`BoardModel`, Unity UI'dan bağımsız bir 4x4 sayı modelidir. Inspector veya
sahne kurulumu gerektirmez. Unity Editor'da `Window > General > Test Runner`
menüsünü açın, `EditMode` sekmesinde `Game2048.Core.EditModeTests` testlerini
çalıştırın.

Sprint 2.1 BoardModel testleri kullanıcı tarafından doğrulandı. Sprint 2.2
sol hareket testleri kullanıcı tarafından doğrulandı. Sprint 2.3 dört yön
testleri de aynı EditMode test assembly'sinde yer alır.

## Sprint 3.1: BoardController kurulumu

Board GameObject'ine `BoardController` ekleyin. Inspector'da `Tile Views`
listesinin hierarchy sırasındaki 16 hücreyi row-major olarak içerdiğini
doğrulayın; boşsa bileşeni Reset ile yeniden ekleyerek otomatik doldurun.

Sprint 3.2 için ScorePanel altındaki `Value` TMP nesnesini BoardController
Inspector'ındaki `Score Text` alanına atayın.

Sprint 3.2 skor takibi kullanıcı tarafından Unity Editor'da doğrulandı.

## Sprint 3.3: Win ve Game Over panelleri

1. `WinPanel` ve `GameOverPanel` nesnelerini Board altına koymayın; Canvas
   altında SafeContent ile aynı seviyede veya Canvas'ın sonunda oluşturun.
2. Her panele yarı saydam Image, TMP mesajı ve `YENİDEN BAŞLA` butonu ekleyin.
   WinPanel metni `2048’E ULAŞTIN!`, GameOverPanel metni `OYUN BİTTİ` olsun;
   her iki panel başlangıçta inactive olmalı.
3. BoardController Inspector'ındaki `Win Panel` ve `Game Over Panel` alanlarına
   doğru nesneleri atayın. Header > NewGameButton ve iki `YENİDEN BAŞLA`
   butonunun OnClick olayına BoardController `StartNewGame()` metodunu bağlayın.
4. Play Mode'da BoardController bağlam menüsündeki `Preview/Test Win State` ve
   `Preview/Test Game Over State` seçenekleriyle panel ve hareket kilidini test edin.

## Sprint 4.1: Klavye kontrolü

Board nesnesine `KeyboardInputController` ekleyin ve `Board Controller`
alanına aynı Board nesnesindeki BoardController bileşenini atayın. Play Mode'da
yön tuşları ile WASD tuşlarını test edin.

Sprint 4.2 swipe kontrolü Unity Editor'da doğrulandı. Android gerçek cihaz
doğrulaması final build aşamasında yapılacak.

## Sprint 5.1: Responsive UI ve Safe Area kurulumu

1. Canvas altında `SafeArea` adlı boş bir UI nesnesi oluşturun. RectTransform
   anchor min `0,0`, anchor max `1,1`, pivot `0.5,0.5` ve dört offset `0` olsun.
   `SafeAreaFitter` bileşenini bu nesneye ekleyin.
2. Mevcut `SafeContent`, `WinPanel` ve `GameOverPanel` nesnelerini SafeArea
   altına taşıyın. WinPanel ve GameOverPanel, SafeContent'ten sonra sıralansın;
   Board altına taşınmasın. `Background` doğrudan Canvas altında, tam ekran
   stretch ve SafeArea dışında kalmaya devam etsin.
3. Board nesnesine `ResponsiveSquareGrid` ekleyin. Mevcut Grid Layout Group'un
   Padding ve Spacing değerlerini değiştirmeyin. Constraint `Fixed Column Count`,
   Constraint Count `4` olarak kalmalı; bileşen Cell Size'ı otomatik hesaplar.
4. Board'a gerekirse `Aspect Ratio Fitter` ekleyin: Aspect Mode
   `Width Controls Height`, Aspect Ratio `1`. Board yatay anchor düzenini ve
   kullanıcının mevcut konum/boşluk ayarlarını koruyun.
5. Cell altındaki TMP `Value` RectTransform'larının dört yönde stretch ve
   offsetlerinin `0` olduğunu doğrulayın. `1024`, `2048`, `4096` preview
   değerlerinde autosize metni hücre içinde tutmalıdır.
6. Game görünümünde `1080x1920`, `720x1280`, `1080x2400` ve dar portrait
   çözünürlükleri test edin. 16 hücrenin kare kaldığını; Header, Board,
   WinPanel ve GameOverPanel'in notch/sistem alanlarına girmediğini doğrulayın.

Sprint 5.1 responsive düzeni, Safe Area hiyerarşisi ve büyük sayı okunabilirliği
kullanıcı tarafından Unity Editor'da doğrulandı.

## Sprint 5.2: Tile animasyonları

BoardController, BoardModel'in ürettiği hareket kayıtlarını kullanarak tile görsellerini
gerçek başlangıç hücresinden hedef hücreye kaydırır. Birleşme kısa bir büyüme,
yeni tile oluşumu scale-in animasyonuyla gösterilir.
Animasyon sırasında yeni hareketler reddedilir. `YENİ OYUN` devam eden animasyonu
durdurur, geçici görselleri siler, hücre ölçeklerini sıfırlar ve yeni oyunu başlatır.

1. Board nesnesindeki `BoardController` bileşeninin `Animation` bölümünde
   `Animations Enabled` açık kalsın.
2. Başlangıç değerleri olarak `Move Animation Duration: 0.10`,
   `Additional Cell Slide Duration: 0.03`, `Maximum Slide Duration: 0.19`,
   `Merge Animation Duration: 0.10` ve `Spawn Animation Duration: 0.12`
   kullanın. Değerler saniyedir ve Inspector'dan ayarlanabilir.
3. Ek GameObject, prefab bağlantısı veya Animator gerekmez. Runtime
   `AnimationLayer` otomatik oluşturulur ve animasyon sonunda silinir. Var olan
   16 `Tile Views` referansını ve hierarchy sırasını koruyun.
4. Play Mode'da bir, iki ve üç hücre ilerleyen tile'ları dört yönde hareket
   ettirin. Geçici tile görsellerinin başlangıç hücresinden hedef hücreye kesintisiz
   kaydığını doğrulayın. Birleşen iki görsel aynı hedefe kaymalı; ardından birleşen
   tile büyüyüp normale dönmeli ve yeni tile sıfır ölçekten açılmalıdır.
5. Bir animasyon sürerken art arda WASD, yön tuşu ve swipe girdileri verin.
   Yalnızca ilk hamle uygulanmalı; animasyon tamamlanınca input tekrar çalışmalıdır.
6. Bir animasyon sürerken `YENİ OYUN` düğmesine basın. Devam eden animasyon
   kesilmeli, skor `0` olmalı, iki yeni tile görünmeli ve görsel konum/ölçek
   bozulması kalmamalıdır.
7. `Animations Enabled` seçeneğini kapatıp aynı hareketleri tekrarlayın. Kayma,
   birleşme ve scale-in görünmemeli; hareket, birleşme, skor, yeni tile, kazanma
   ve Game Over kuralları çalışmaya devam etmelidir.
8. Test Runner > EditMode altında mevcut oyun kurallarıyla birlikte hareket
   kayıtlarını doğrulayan toplam `36` testin geçtiğini kontrol edin.

Sprint 5.2 hareket, birleşme ve spawn animasyonları kullanıcı tarafından Unity
Editor'da doğrulandı.

## Sprint 6.1: High score kaydı

BoardController mevcut oyun skorunu `Game2048.HighScore` PlayerPrefs anahtarıyla
karşılaştırır. Skor kayıtlı değeri geçtiğinde high score hemen kaydedilir ve
`EN İYİ` alanı güncellenir. Normal New Game ve paneldeki yeniden başlatma
butonları high score değerini silmez.

1. Board nesnesindeki `BoardController` bileşenini seçin.
2. Header > HighScorePanel > Value TMP nesnesini `High Score Text` alanına
   atayın. Mevcut `Score Text`, panel ve Tile Views referanslarını değiştirmeyin.
3. Play Mode'u başlatın. `SKOR` değeri `0` olurken `EN İYİ` kayıtlı değeri
   göstermelidir.
4. Birleşmeler yaparak mevcut skoru `EN İYİ` değerinin üzerine çıkarın. `EN İYİ`
   aynı hamlede yeni skora yükselmelidir.
5. Header veya sonuç panellerindeki New Game/Restart butonuna basın. `SKOR` `0`
   olmalı, `EN İYİ` korunmalıdır.
6. Play Mode'dan çıkıp yeniden girin. Ardından Unity Editor'ı kapatıp projeyi
   yeniden açarak kayıtlı high score değerinin yeniden yüklendiğini doğrulayın.
7. Test Runner > EditMode altında toplam `41` testin geçtiğini ve Console'da
   compiler error bulunmadığını kontrol edin.

Sprint 6.1 high score kaydı, New Game sırasında korunma, PlayerPrefs yükleme ve
`EN İYİ` TMP bağlantısı kullanıcı tarafından Unity Editor'da doğrulandı.

## Audio Polish ara görevi

Gerekli altı WAV dosyası `Assets/Audio` klasöründedir: `move.wav`,
`merge.wav`, `spawn.wav`, `button_click.wav`, `win_2048.wav` ve
`game_over.wav`.

1. Board nesnesine `GameAudioController` ekleyin. `RequireComponent` aynı
   nesneye otomatik olarak bir `AudioSource` ekler. AudioSource üzerinde
   `Play On Awake` ve `Loop` kapalı, `Spatial Blend` ise `0` olmalıdır.
2. GameAudioController Inspector alanlarına sırasıyla `Move`, `Merge`, `Spawn`,
   `Button Click`, `Win 2048` ve `Game Over` WAV dosyalarını atayın. Her sesin
   seviyesini yanındaki Volume alanından ayarlayın.
3. BoardController içindeki `Audio Controller` alanına aynı Board nesnesindeki
   GameAudioController bileşenini atayın.
4. Header içindeki `NewGameButton` ile WinPanel ve GameOverPanel içindeki
   `YENİDEN BAŞLA` butonlarının her birinde OnClick listesine
   `GameAudioController.PlayButtonClick()` ekleyin. Var olan
   `BoardController.StartNewGame()` bağlantılarını kaldırmayın.
5. Play Mode'da birleşmesiz geçerli hareketin move, birleşmenin merge ve yeni
   tile'ın spawn sesi verdiğini doğrulayın. Geçersiz hamlede move sesi
   çalmamalıdır.
6. `Preview/Test Win State` ve `Preview/Test Game Over State` menülerini
   kullanarak sonuç seslerinin bir kez çaldığını, sonuçtan sonraki inputların
   sesi tekrarlamadığını ve terminal durumda spawn sesinin çalmadığını kontrol
   edin.
7. Üç New Game/Restart butonunun click sesi verdiğini, oyunu sıfırladığını ve
   başlangıç tile'ları için spawn sesinin çalıştığını doğrulayın.
8. BoardController üzerindeki `Animations Enabled` seçeneğini kapatıp move,
   merge, spawn, win ve Game Over seslerinin çalışmaya devam ettiğini sınayın.
9. Bir AudioClip alanını geçici olarak boşaltın. İlgili olay tekrarlandığında
   NullReferenceException oluşmamalı ve eksik klip uyarısı yalnızca bir kez
   yazılmalıdır. Testten sonra klibi yeniden atayın.
10. Test Runner > EditMode altında mevcut toplam `41` testin geçtiğini ve
    Console'da compiler error bulunmadığını kontrol edin.

Audio Polish ara görevi; move, merge, spawn, click, win ve Game Over sesleriyle
birlikte kullanıcı tarafından Unity Editor'da doğrulandı.

## Sprint 6.2: Test, temizlik ve Windows/Android build

Sprint 6.2 final denetim ve build doğrulaması devam ediyor `[-]`. Kod ve proje
dosyaları üzerinde yapılan statik kontrolde aşağıdaki durumlar doğrulandı:

- Proje sürümü Unity `6000.6.2f1`.
- `Assets/Scenes/SampleScene.unity`, build sahne listesinde etkin.
- Ürün adı `2048` ve Input Handling değeri `Input System Package (New)`.
- BoardController'ın 16 TileView, skor, high score, sonuç paneli ve ses
  referansları dolu. Klavye ve swipe bileşenleri aynı BoardController'a bağlı.
- Altı AudioClip ve üç New Game/Restart butonunun ses bağlantıları mevcut.
- Sahne ve Tile prefabında `Missing Script` kaydı bulunmuyor.
- Assets altındaki proje dosya ve klasörlerinde eksik `.meta` bulunmuyor.
- Android ayarlarında IL2CPP, ARM64, Minimum API Level 26 ve Target API
  `Automatic (highest installed)` karşılıkları kayıtlı.

Final build öncesinde Unity Editor'da aşağıdaki ayarları uygulayın. Mevcut
Player Settings dosyasında varsayılan yön Landscape Right ve masaüstü çözünürlüğü
`1920x1080` olduğundan bu iki madde zorunludur.

### Ortak Unity Editor kontrolleri

1. Projeyi Unity `6000.6.2f1` ile açın ve asset import işleminin bitmesini
   bekleyin. Console'u temizleyip scriptlerin yeniden derlenmesini bekleyin;
   kırmızı compiler error kalmamalıdır.
2. `File > Build Profiles` penceresinde Scene List bölümünü açın.
   `Assets/Scenes/SampleScene.unity` etkin ve ilk sahne olmalıdır. Başka test
   veya template sahnesini build listesine eklemeyin.
3. `Edit > Project Settings > Player` altında Product Name değerini `2048`
   olarak doğrulayın. `Active Input Handling`, `Input System Package (New)`
   olarak kalmalıdır.
4. `Resolution and Presentation` altında Default Orientation değerini
   `Portrait` yapın. `Auto Rotation` seçilirse yalnızca Portrait yönüne izin
   verin; final build için sabit `Portrait` tercih edilir.
5. Board nesnesinde 16 Tile Views, Score Text, High Score Text, Win Panel,
   Game Over Panel ve Audio Controller alanlarının dolu olduğunu doğrulayın.
   KeyboardInputController ve SwipeInputController içindeki Board Controller
   alanları da aynı BoardController'ı göstermelidir.
6. SafeArea altında SafeContent, WinPanel ve GameOverPanel sırasını; Board
   üzerindeki ResponsiveSquareGrid, AspectRatioFitter ve Grid Layout Group
   bileşenlerini kontrol edin. AudioSource üzerinde Play On Awake ve Loop
   kapalı, Spatial Blend `0` olmalıdır.
7. `Window > General > Test Runner > EditMode` altında bütün `41` testi
   çalıştırın. Sonucu kaydetmeden Sprint 6.2 kabul kriterlerini tamamlanmış
   saymayın.

### Windows build ve test

1. `File > Build Profiles` içinde bir Windows profili oluşturun veya mevcut
   profili seçin. Platform `Windows`, mimari `x86_64` olsun.
2. Player Settings > Resolution and Presentation altında Fullscreen Mode'u
   `Windowed`, Default Screen Width değerini `720`, Height değerini `1280`
   yapın. Resizable Window kapalı kalabilir.
3. Development Build, Script Debugging ve Autoconnect Profiler seçeneklerini
   final build için kapatın. Çıktıyı `Builds/Windows` altına alın; bu klasörü
   Git'e eklemeyin.
4. Oluşan `.exe` dosyasını çalıştırın. Yeni oyunun iki tile ve sıfır skorla
   açıldığını; WASD, yön tuşları ve mouse drag hareketlerinin çalıştığını;
   geçersiz hamlenin tile veya ses üretmediğini doğrulayın.
5. Birleşme skoru, EN İYİ kaydı, New Game/Restart, Win, Game Over, animasyon
   sırasındaki input kilidi ve bütün sesleri sınayın. Uygulamayı kapatıp yeniden
   açarak high score'un yüklendiğini kontrol edin.
6. `1080x1920`, `720x1280`, `1080x2400` ve dar portrait pencere oranlarında
   Board'un kare, metinlerin okunabilir ve panellerin çakışmasız kaldığını
   doğrulayın.

### Android build ve gerçek cihaz testi

1. Unity Hub üzerinden bu Editor sürümü için Android Build Support, Android
   SDK & NDK Tools ve OpenJDK modüllerinin kurulu olduğunu doğrulayın.
2. Build Profiles içinde Android profili oluşturup platformu Android'e
   geçirin. Scene List içinde yalnızca etkin SampleScene bulunmalıdır.
3. Player Settings değerlerini kontrol edin:
   - Product Name: `2048`
   - Default Orientation: `Portrait`
   - Active Input Handling: `Input System Package (New)`
   - Scripting Backend: `IL2CPP`
   - Target Architectures: `ARM64`
   - Minimum API Level: `Android 8.0 (API 26)`
   - Target API Level: `Automatic (highest installed)`
   - Version: `1.0`, Bundle Version Code: `1`
4. Android Application Identifier için size ait benzersiz ters alan adı
   kullanın; örneğin `com.oguzirmak.game2048`. Projede Android'e özel kimlik
   henüz kayıtlı olmadığından bu değer build öncesinde mutlaka girilmelidir.
5. Doğrudan cihaz testi için Build App Bundle seçeneğini kapatıp APK alın.
   Mağaza adayı için seçeneği açıp AAB alın. Çıktıyı `Builds/Android` altında
   üretin; `.apk`, `.aab` ve Builds klasörünü Git'e eklemeyin.
6. APK'yı gerçek cihaza kurun. Portrait yönünü, notch/sistem çubuğu Safe Area
   davranışını ve `1080x2400` benzeri uzun ekran oranını kontrol edin.
7. Dört swipe yönünü, kısa tap ve kısa sürüklemenin hamle oluşturmamasını,
   her swipe'ın tek hamle üretmesini ve UI butonlarının çalışmasını sınayın.
8. Geçerli/geçersiz hareket, birleşme, skor, EN İYİ, restart, Win, Game Over,
   animasyon input kilidi ve tüm sesleri cihaz hoparlörüyle doğrulayın.
   Uygulamayı tamamen kapatıp açarak high score'un korunduğunu kontrol edin.

### Git ve final commit öncesi kontrol

1. `git status --short` ile bütün değişiklikleri inceleyin. Aşağıdaki kaynaklar
   ve karşılık gelen klasör/dosya `.meta` dosyaları final commit'e girmelidir:
   - `Assets/Scenes`, `Assets/Prefabs`, `Assets/Scripts`, `Assets/Tests`
   - `Assets/Audio` içindeki altı WAV ve meta dosyaları
   - `Assets/TextMesh Pro` Essential Resources ve meta dosyaları
   - `Assets/Settings` içindeki kullanılan URP/Input System varlıkları
   - `Packages/manifest.json` ve `Packages/packages-lock.json`; rounded-corner
     paket bağımlılığı bu iki dosya üzerinden korunur
   - `ProjectSettings`, `.gitignore`, `.gitattributes`, README ve proje planı
   - Build Profile asset'i Assets altında oluşturulduysa kendisi ve `.meta` dosyası
2. `Library`, `Temp`, `Logs`, `obj`, `Build`, `Builds` ve `UserSettings`
   klasörleri ile `.apk` ve `.aab` çıktılarının listede olmadığını doğrulayın.
3. WAV, font ve görsel dosyaları `.gitattributes` nedeniyle Git LFS kullanır.
   `git lfs status` hatasız çalışmadan final stage/commit işlemine geçmeyin.
4. Stage işleminden sonra `git diff --cached --check` ve `git status --short`
   çalıştırın. Unity'nin ürettiği `.csproj`, `.sln` ve cache dosyalarını eklemeyin.
5. Windows build, Android cihaz testi ve 41 EditMode testi tamamlanmadan Sprint
   6.2 durumunu `[x]` yapmayın.
