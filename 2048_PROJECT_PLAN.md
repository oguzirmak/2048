# Unity 2048 - Proje Roadmap ve Sprint Planı

Bu dosya Unity ile geliştirilecek klasik 2048 oyununun kapsamını, teknik kararlarını, phase ve sprintlerini, kabul kriterlerini ve Codex çalışma kurallarını içerir.

Codex her görevde önce bu dosyayı okumalı ve yalnızca kullanıcı tarafından belirtilen sprinti uygulamalıdır.

---

## 1. Proje Özeti

### Amaç

Unity kullanılarak Windows ve Android platformlarında çalışabilen klasik bir 2048 oyunu geliştirmek.

### Hedef platformlar

- Windows: klavye kontrolü
- Android: dört yönlü swipe kontrolü
- Öncelikli ekran yönü: Portrait

### Temel özellikler

- 4x4 oyun tahtası
- Oyun başlangıcında iki sayı
- Boş hücrelere rastgele `2` veya `4` eklenmesi
- Yukarı, aşağı, sağa ve sola hareket
- Eşit sayıların birleşmesi
- Bir kutunun aynı hamlede yalnızca bir kez birleşmesi
- Skor sistemi
- `2048` sayısına ulaşınca kazanma
- Hamle kalmayınca Game Over
- New Game / Restart
- Klavye ve mobil swipe kontrolü
- High score kaydı
- Android build

### İlk sürümde kapsam dışı

- Online leaderboard
- Kullanıcı hesabı
- Reklam ve uygulama içi satın alma
- Çok oyunculu mod
- Karmaşık tema sistemi
- Cloud save
- Undo sistemi
- Gelişmiş animasyon framework'ü

---

## 2. Teknik Yaklaşım

### Önerilen klasör yapısı

```text
Assets/
├── Scenes/
├── Scripts/
│   ├── Core/
│   ├── UI/
│   └── Input/
├── Prefabs/
├── Sprites/
├── Materials/
└── Audio/
```

### Önerilen sorumluluklar

| Bileşen | Sorumluluk |
|---|---|
| `BoardModel` | 4x4 sayı matrisi, hareket, birleşme ve oyun kuralları |
| `BoardController` | Model, görünüm ve input arasındaki koordinasyon |
| `BoardView` | Tahtadaki 16 hücreyi ekranda güncelleme |
| `TileView` | Tek hücrenin sayı, renk ve yazı görünümü |
| `GameManager` | Skor, oyun durumu, kazanma, kaybetme ve restart |
| `KeyboardInputController` | Klavye hareketlerini oyun yönlerine dönüştürme |
| `SwipeInputController` | Dokunmatik swipe hareketlerini oyun yönlerine dönüştürme |

### Temel veri modeli

```csharp
private readonly int[,] board = new int[4, 4];
```

`0`, boş hücre anlamına gelir.

### Önemli oyun kuralları

1. Oyun iki rastgele kutuyla başlar.
2. Yeni kutu boş bir hücreye yerleştirilir.
3. Yeni kutu çoğunlukla `2`, daha düşük ihtimalle `4` olur.
4. Yeni kutu yalnızca tahtayı değiştiren geçerli hamleden sonra oluşur.
5. Bir kutu aynı hamlede yalnızca bir kez birleşebilir.
6. Birleşme sonucu oluşan değer skora eklenir.
7. Boş hücre yoksa ve hiçbir komşu birleşemiyorsa oyun biter.
8. Herhangi bir hücre `2048` olduğunda kazanma durumu oluşur.

### Kritik birleşme örnekleri

```text
2 0 2 4  -> 4 4 0 0
2 2 2 2  -> 4 4 0 0
2 2 4 0  -> 4 4 0 0
4 4 8 8  -> 8 16 0 0
2 2 4 4  -> 4 8 0 0
4 4 4 0  -> 8 4 0 0
```

---

## 3. Global Codex Kuralları

Codex bütün sprintlerde aşağıdaki kuralları uygulamalıdır:

1. Önce `2048_PROJECT_PLAN.md` dosyasını ve mevcut Unity projesini incele.
2. Kullanılan Unity sürümünü `ProjectSettings/ProjectVersion.txt` üzerinden belirle.
3. Yalnızca istenen sprinti uygula; gelecek sprintlerin özelliklerini şimdiden ekleme.
4. Mevcut kullanıcı değişikliklerini koru ve ilgisiz dosyaları değiştirme.
5. `Library`, `Temp`, `Logs`, `obj`, `Build` ve `Builds` klasörlerine dokunma.
6. Gereksiz paket veya üçüncü taraf bağımlılık ekleme.
7. Unity scene ve prefab YAML dosyalarını doğrudan düzenlemek riskliyse düzenleme. Bunun yerine gerekli Editor ve Inspector adımlarını açıkça yaz.
8. UI metinlerinde mümkün olduğunda TextMeshPro kullan.
9. Kodları küçük sorumluluklara böl; ancak mevcut sprint için gereksiz mimari kurma.
10. Public alanları yalnızca gerçekten dış erişim gerekiyorsa kullan. Inspector alanlarında `[SerializeField] private` tercih et.
11. Unity'nin mevcut sürümünde kullanım dışı bırakılmış API'lerden kaçın.
12. Console hatasına neden olacak yarım referans veya eksik class bırakma.
13. Uygulanabiliyorsa saf C# oyun mantığını Unity görünüm kodundan ayır.
14. Her sprint sonunda değiştirilen dosyaları, manuel Unity adımlarını ve test yöntemini bildir.

### Sprint sonu çıktı formatı

Codex her sprint sonunda şunları raporlamalıdır:

```text
1. Tamamlanan işler
2. Oluşturulan/değiştirilen dosyalar
3. Unity Editor içinde yapılacak manuel işlemler
4. Kabul kriterlerinin test adımları
5. Derleme/test sonucu
6. Bilinen eksikler veya sonraki sprint bağımlılıkları
```

---

## 4. Definition of Done

Bir sprint ancak şu şartlarda tamamlanmış sayılır:

- Sprint kapsamındaki bütün maddeler uygulanmıştır.
- Unity Console'da yeni compiler error yoktur.
- Kabul kriterleri elle veya otomatik testlerle doğrulanmıştır.
- Inspector'da yapılması gereken işlemler tamamlanmıştır.
- İlgisiz dosyalar değiştirilmemiştir.
- Kod anlaşılır isimlere ve tek sorumluluğa sahiptir.
- Çalışan durum Git commit'i olarak kaydedilmiştir.

---

## 5. Sprint Durum Tablosu

| Phase | Sprint | Durum | Açıklama |
|---|---|---|---|
| 0 | 0.1 | [x] | Proje hazırlığı Unity Editor'da doğrulandı |
| 1 | 1.1 | [x] | Statik Canvas ve 4x4 tahta Unity Editor'da doğrulandı |
| 1 | 1.2 | [x] | Tile görünüm sistemi Unity Editor'da doğrulandı |
| 2 | 2.1 | [x] | Board veri modeli ve rastgele kutu Unity Test Runner'da doğrulandı |
| 2 | 2.2 | [x] | Sola hareket ve birleşme Unity Test Runner'da doğrulandı |
| 2 | 2.3 | [x] | Dört yönlü hareket Unity Test Runner'da doğrulandı |
| 3 | 3.1 | [x] | BoardModel ve TileView bağlantısı Unity Editor'da doğrulandı |
| 3 | 3.2 | [x] | Skor sistemi Unity Editor'da doğrulandı |
| 3 | 3.3 | [x] | Kazanma, Game Over ve Restart Unity Editor'da doğrulandı |
| 4 | 4.1 | [-] | Klavye kontrolü uygulanıyor; Unity Editor doğrulaması bekleniyor |
| 4 | 4.2 | [x] | Mobil swipe kontrolü Unity Editor'da doğrulandı |
| 5 | 5.1 | [x] | Responsive UI ve görsel tasarım Unity Editor'da doğrulandı |
| 5 | 5.2 | [x] | Hareket, birleşme ve spawn animasyonları Unity Editor'da doğrulandı |
| 6 | 6.1 | [x] | High score kaydı Unity Editor'da doğrulandı |
| Ara | Audio Polish | [x] | Ses geri bildirimleri Unity Editor'da doğrulandı |
| 6 | 6.2 | [-] | Final denetim ve build doğrulaması devam ediyor |

---

# PHASE 0 - Proje Hazırlığı

## Sprint 0.1 - Unity Projesinin Hazırlanması

### Hedef

Unity projesini güvenli ve düzenli geliştirmeye hazır hâle getirmek.

### Görevler

- Unity sürümünü belirle.
- Temel klasör yapısını oluştur.
- `.gitignore` dosyasını doğrula.
- Unity tarafından üretilen klasörlerin Git'e eklenmesini engelle.
- Proje yapısını açıklayan kısa `README.md` oluştur.
- Henüz gameplay, UI veya algoritma yazma.

### Kabul kriterleri

- Proje Unity Editor'da açılmalıdır.
- Console'da compiler error olmamalıdır.
- Gerekli klasörler bulunmalıdır.
- `Library`, `Temp`, `Logs`, `obj`, `Build`, `Builds` Git tarafından izlenmemelidir.

### Önerilen commit

```text
chore: initialize Unity 2048 project
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 0.1'i uygula. Gelecek sprintlerin gameplay veya UI
özelliklerini ekleme. Sprint sonunda belirtilen rapor formatını kullan.
```

---

# PHASE 1 - Statik Oyun Arayüzü

## Sprint 1.1 - Canvas ve 4x4 Tahta

### Hedef

Portrait ekranda responsive çalışan statik 4x4 oyun ekranını hazırlamak.

### Görevler

- Canvas yapısını planla.
- Başlık alanı oluştur.
- Score ve High Score alanları oluştur.
- New Game butonu oluştur.
- Oyun tahtası arka planı oluştur.
- `Grid Layout Group` ile 16 eşit hücre düzeni oluştur.
- Farklı ekran oranları için `Canvas Scaler` ayarlarını belirle.
- Gerekirse manuel Editor adımlarını ayrıntılı yaz.

### Kabul kriterleri

- [x] Game ekranında 16 eşit hücre görünmelidir.
- [x] Hücreler 4 satır ve 4 sütun hâlinde olmalıdır.
- [x] UI elemanları üst üste binmemelidir.
- [x] New Game butonu tıklanabilir olmalıdır.
- [x] Ekran oranı değiştiğinde temel düzen bozulmamalıdır.

### Önerilen commit

```text
feat: create responsive 2048 game board UI
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 1.1'i uygula. Henüz oyun algoritması, rastgele kutu,
hareket veya skor mantığı ekleme. Scene/prefab dosyasını güvenle
düzenleyemiyorsan gerekli Unity Editor adımlarını eksiksiz yaz.
```

## Sprint 1.2 - Tile Görünüm Sistemi

### Hedef

Tek bir hücrenin değerini, rengini ve metnini yönetebilen tekrar kullanılabilir görünüm oluşturmak.

### Görevler

- `TileView.cs` oluştur.
- Sayı değerini TextMeshPro ile göster.
- `0` değerinde yazıyı gizle.
- Temel değer-renk eşlemesini oluştur.
- Büyük sayılarda font boyutunun küçülebilmesini destekle.
- Tile prefab kurulumu için manuel adımları belirt.

### Kabul kriterleri

- [x] `TileView`, verilen değeri doğru göstermelidir.
- [x] `0`, boş hücre görünümü vermelidir.
- [x] `2`, `4`, `8`, `16` gibi değerler farklı renklere sahip olmalıdır.
- [x] Inspector referansları eksikse anlaşılır hata verilmelidir.

### Önerilen commit

```text
feat: add reusable tile view component
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 1.2'yi uygula. Board hareketi veya oyun kuralları ekleme.
TileView ve gerekli görünüm yapılandırmasına odaklan.
```

---

# PHASE 2 - Temel Oyun Algoritması

## Sprint 2.1 - Board Veri Modeli ve Rastgele Kutu

### Hedef

Unity UI'dan bağımsız çalışan 4x4 tahta modelini oluşturmak.

### Görevler

- `BoardModel` oluştur.
- 4x4 `int[,]` veri yapısını kullan.
- Tahtayı sıfırlama işlemi ekle.
- Hücre değeri okuma yöntemini ekle.
- Boş hücreleri bul.
- Rastgele boş hücre seç.
- Yeni oyunda iki kutu oluştur.
- Yeni değerleri çoğunlukla `2`, daha düşük ihtimalle `4` olarak üret.
- Mümkünse saf C# mantığı için test ekle.

### Kabul kriterleri

- [x] Yeni tahta tamamen boş başlayabilmelidir.
- [x] Yeni oyun başlatıldığında iki farklı hücre dolmalıdır.
- [x] Dolu hücre üzerine yeni sayı yazılmamalıdır.
- [x] Tam dolu tahtada yeni sayı üretme denemesi güvenli biçimde başarısız olmalıdır.

### Önerilen commit

```text
feat: add board model and random tile spawning
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 2.1'i uygula. Henüz hareket veya birleşme algoritması
ekleme. BoardModel mantığını mümkün olduğunca Unity görünümünden bağımsız tut.
```

## Sprint 2.2 - Sola Hareket ve Birleşme

### Hedef

2048'in sıkıştır-birleştir-sıkıştır algoritmasını yalnızca sol yön için doğru uygulamak.

### Görevler

- Satırdaki sıfırları kaldırarak sola sıkıştır.
- Yan yana eşit değerleri birleştir.
- Birleşen hücrenin aynı hamlede yeniden birleşmesini engelle.
- Birleşmeden sonra tekrar sola sıkıştır.
- Hareketin tahtayı değiştirip değiştirmediğini döndür.
- Birleşmelerden gelen skor artışını sonuç olarak üret.
- Kritik örnekler için test ekle.

### Zorunlu test örnekleri

```text
2 0 2 4  -> 4 4 0 0
2 2 2 2  -> 4 4 0 0
2 2 4 0  -> 4 4 0 0
4 4 4 0  -> 8 4 0 0
4 4 8 8  -> 8 16 0 0
```

### Kabul kriterleri

- [x] Zorunlu test örneklerinin tamamı geçmelidir.
- [x] Bir hücre aynı hamlede ikinci defa birleşmemelidir.
- [x] Değişiklik yoksa hareket sonucu geçersiz olarak bildirilmelidir.

### Önerilen commit

```text
feat: implement left movement and tile merging
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 2.2'yi uygula. Sol hareket algoritmasını ve belirtilen
testleri ekle. Diğer yönleri veya input sistemini henüz ekleme.
```

## Sprint 2.3 - Dört Yönlü Hareket

### Hedef

Sol hareket kuralını bozmadan dört yönün tamamını desteklemek.

### Görevler

- Hareket yönünü temsil eden açık bir enum oluştur.
- Sola, sağa, yukarı ve aşağı hareketi uygula.
- Bütün yönlerde aynı birleşme kurallarını koru.
- Her hareket için değişiklik ve skor sonucunu döndür.
- Dört yön için testler ekle.

### Kabul kriterleri

- [x] Dört yön doğru çalışmalıdır.
- [x] Sağ ve dikey hareketlerde sıra bozulmamalıdır.
- [x] Geçersiz hareket tahtayı ve skoru değiştirmemelidir.
- [x] Testler birbirinden bağımsız çalışmalıdır.

### Önerilen commit

```text
feat: support movement in all four directions
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 2.3'ü uygula. Mevcut sol hareket davranışını koruyarak
dört yön desteği ve ilgili testleri ekle. Henüz keyboard veya touch input ekleme.
```

---

# PHASE 3 - Oyun Döngüsü

## Sprint 3.1 - Geçerli Hamle ve Yeni Kutu Üretimi

### Hedef

Tahta modeli ile görünümü bağlamak ve yalnızca geçerli hareketten sonra yeni kutu oluşturmak.

### Görevler

- `BoardController` oluştur.
- Tahta modelini başlat.
- Modeldeki değerleri 16 `TileView` üzerinde göster.
- Bir hareket isteğini modele gönder.
- Tahta değiştiyse bir yeni kutu üret.
- Geçersiz hamlede yeni kutu üretme.
- Her işlemden sonra görünümü yenile.

### Kabul kriterleri

- [x] Yeni oyunda iki kutu görünmelidir.
- [x] Geçerli hamleden sonra yalnızca bir yeni kutu oluşmalıdır.
- [x] Geçersiz hamlede yeni kutu oluşmamalıdır.
- [x] UI her hamleden sonra modelle aynı değerleri göstermelidir.

### Önerilen commit

```text
feat: connect board model to tile views
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 3.1'i uygula. BoardModel, BoardController ve TileView
entegrasyonuna odaklan. Skor, kazanma, Game Over ve input ekleme.
```

## Sprint 3.2 - Skor Sistemi

### Hedef

Birleşmelerden oluşan skorun doğru hesaplanması ve UI'da gösterilmesi.

### Görevler

- Oyun skorunu sakla.
- Birleşme sonuçlarını skora ekle.
- Score Text'i güncelle.
- New Game işleminde skoru sıfırla.

### Skor örnekleri

```text
2 + 2 = 4   -> +4 puan
4 + 4 = 8   -> +8 puan
8 + 8 = 16  -> +16 puan
```

### Kabul kriterleri

- [x] Aynı hamlede birden fazla birleşme varsa toplam skor doğru olmalıdır.
- [x] Geçersiz hamle skoru değiştirmemelidir.
- [x] New Game skoru sıfırlamalıdır.

### Önerilen commit

```text
feat: add score tracking and score UI
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 3.2'yi uygula. Birleşme skorunu oyun durumuna ve mevcut
score UI alanına bağla. High score'u henüz ekleme.
```

## Sprint 3.3 - Kazanma, Game Over ve Restart

### Hedef

Oyunun temel durum geçişlerini tamamlamak.

### Görevler

- `2048` değerini algıla.
- Kazanma panelini göster.
- Boş hücre ve olası birleşme kontrolü yap.
- Hamle kalmadığında Game Over panelini göster.
- New Game/Restart butonunu bağla.
- Oyun bittikten sonra hareketleri engelle.

### Kabul kriterleri

- [x] `2048` oluştuğunda kazanma durumu tetiklenmelidir.
- [x] Tahta dolu olsa bile birleşme varsa oyun devam etmelidir.
- [x] Tahta dolu ve birleşme yoksa Game Over olmalıdır.
- [x] Restart yeni, temiz bir oyun başlatmalıdır.

### Önerilen commit

```text
feat: add win game-over and restart flow
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 3.3'ü uygula. Kazanma, Game Over ve Restart akışlarını
ekle. Mobil input, animasyon veya kayıt sistemi ekleme.
```

---

# PHASE 4 - Kontroller

## Sprint 4.1 - Klavye Kontrolü

### Hedef

Windows ve Unity Editor üzerinde tek hamlelik güvenilir klavye kontrolü sağlamak.

### Görevler

- Yön tuşlarını destekle.
- İsteğe bağlı WASD desteği ekle.
- Bir tuş basımını yalnızca bir hamleye dönüştür.
- Oyun bitmişse input gönderme.
- Projenin etkin Input Handling ayarına uygun API kullan.

### Kabul kriterleri

- Dört yön doğru hareket etmelidir.
- Bir tuşa bir kez basmak tek hamle oluşturmalıdır.
- UI butonları çalışmaya devam etmelidir.

### Önerilen commit

```text
feat: add keyboard controls
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 4.1'i uygula. Mevcut Unity sürümünü ve Active Input
Handling ayarını dikkate alarak yön tuşu ve WASD kontrolü ekle.
Mobil swipe ekleme.
```

## Sprint 4.2 - Mobil Swipe Kontrolü

### Hedef

Android cihazlarda dört yönlü güvenilir swipe kontrolü sağlamak.

### Görevler

- Dokunma başlangıç konumunu kaydet.
- Dokunma bitiş konumunu kaydet.
- Yatay/dikey baskın yönü belirle.
- Minimum swipe mesafesi kullan.
- Küçük dokunmaları hamle sayma.
- UI butonuna yapılan dokunmanın oyun hamlesine dönüşmesini engelle.
- Mouse drag ile Editor testi mümkünse destekle.

### Kabul kriterleri

- [x] Dört yöne swipe doğru çalışmalıdır.
- [x] Tek swipe yalnızca bir hamle üretmelidir.
- [x] Tap hareketi hamle oluşturmamalıdır.
- [x] New Game ve diğer UI butonları normal çalışmalıdır.

### Önerilen commit

```text
feat: add mobile swipe controls
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 4.2'yi uygula. Mobil swipe algılama ekle; minimum mesafe,
baskın yön ve UI etkileşimi kontrollerini uygula. Gameplay kurallarını değiştirme.
```

---

# PHASE 5 - Görsel İyileştirme

## Sprint 5.1 - Responsive UI ve Görsel Tasarım

### Hedef

Oyunu farklı telefon ekranlarında okunabilir ve tutarlı göstermek.

### Görevler

- Klasik 2048'e benzer, özgün bir renk paleti kullan.
- Büyük sayılarda font boyutunu ayarla.
- Portrait Canvas düzenini iyileştir.
- Safe Area desteği ekle.
- Buton ve panellerin okunabilirliğini kontrol et.

### Kabul kriterleri

- [x] 16 hücre farklı ekranlarda kare görünmelidir.
- [x] Büyük sayılar hücre dışına taşmamalıdır.
- [x] Notch ve sistem çubukları önemli UI alanlarını kapatmamalıdır.

### Önerilen commit

```text
style: polish responsive 2048 UI
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 5.1'i uygula. Mevcut çalışan gameplay kodunu değiştirmeden
responsive UI, Safe Area, renk ve font iyileştirmelerine odaklan.
```

## Sprint 5.2 - Basit Animasyonlar

### Hedef

Oyun geri bildirimini küçük ve güvenli animasyonlarla iyileştirmek.

### Görevler

- Yeni kutu oluştuğunda kısa scale animasyonu.
- Birleşen kutuda kısa vurgu animasyonu.
- Animasyon sırasında input kilitlenmesi gerekip gerekmediğini değerlendir.
- Animasyonların tahta modelini değiştirmemesini sağla.

### Kabul kriterleri

- [x] Animasyonlar sayı ve konum tutarlılığını bozmamalıdır.
- [x] Hızlı input, çift hamle veya görsel bozulma üretmemelidir.
- [x] Animasyon kapatılsa oyun mantığı çalışmaya devam etmelidir.

### Önerilen commit

```text
feat: add tile spawn and merge animations
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 5.2'yi uygula. Oyun modelini değiştirmeden yeni kutu ve
birleşme için küçük animasyonlar ekle. Harici tween paketi ekleme.
```

---

# PHASE 6 - Kayıt, Test ve Build

## Sprint 6.1 - High Score Kaydı

### Hedef

En yüksek skorun uygulama kapatılıp açıldığında korunması.

### Görevler

- High score değerini tut.
- Skor high score'u geçtiğinde güncelle.
- `PlayerPrefs` ile kaydet ve yükle.
- High Score Text'i güncelle.
- Normal restart işleminde high score'u silme.

### Kabul kriterleri

- [x] High score doğru güncellenmelidir.
- [x] Oyun yeniden başlatıldığında korunmalıdır.
- [x] Uygulama kapatılıp açıldığında yüklenmelidir.

### Önerilen commit

```text
feat: persist high score with PlayerPrefs
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 6.1'i uygula. PlayerPrefs ile high score kaydı ve UI
entegrasyonu ekle. Aktif oyun tahtasını kaydetme.
```

## Ara Görev - Audio Polish

### Durum

[x]

### Hedef

Mevcut oyun akışına, animasyonlara ve kurallara dokunmadan temel ses geri
bildirimlerini eklemek.

### Görevler

- Hareket, birleşme, yeni tile, buton, kazanma ve Game Over seslerini yönet.
- Sesleri tek bir `AudioSource` üzerinden `PlayOneShot` ile oynat.
- Ses kliplerini ve ses seviyelerini Inspector'dan ayarlanabilir tut.
- Geçersiz hamlede hareket sesi oynatma.
- Kazanma ve Game Over durumlarında spawn sesini bastır.
- Oyun bittikten sonra sonuç seslerinin tekrarlanmasını engelle.
- Mevcut New Game, animasyon, skor ve input davranışlarını koru.

### Kabul kriterleri

- [x] Geçerli birleşmesiz hamlede hareket sesi, birleşmede birleşme sesi çalmalıdır.
- [x] Yeni tile oluştuğunda spawn sesi çalmalıdır.
- [x] Kazanma ve Game Over sesleri doğru anda bir kez çalmalıdır.
- [x] Geçersiz hamlede hareket sesi çalmamalıdır.
- [x] Üç New Game/Restart butonunda buton sesi çalışmalıdır.
- [x] Eksik AudioClip referansı oyunu durdurmamalı ve yalnızca bir kez uyarı vermelidir.
- [x] Sesler animasyonlar açıkken ve kapalıyken doğru çalışmalıdır.

## Sprint 6.2 - Test, Temizlik ve Android Build

### Hedef

İlk yayınlanabilir build'i üretmeye hazır, hatasız bir sürüm oluşturmak.

### Görevler

- Compiler warning ve error'ları incele.
- Kullanım dışı Unity API'lerini güncelle.
- Kritik algoritma testlerini çalıştır.
- Eksik Inspector referanslarını belirle.
- Portrait orientation ayarlarını kontrol et.
- Android build için manuel kontrol listesi oluştur.
- Build klasörünün Git tarafından izlenmediğini doğrula.

### Kabul kriterleri

- Unity Console'da compiler error olmamalıdır.
- Kritik birleşme testleri geçmelidir.
- Windows/Editor kontrolleri çalışmalıdır.
- Android swipe ve UI kontrolleri cihazda test edilebilmelidir.
- APK/AAB build alınabilmelidir.

### Önerilen commit

```text
chore: prepare first Android release build
```

### Codex komutu

```text
2048_PROJECT_PLAN.md dosyasını oku ve Global Codex Kurallarına uy.
Yalnızca Sprint 6.2'yi uygula. Projeyi incele, testleri çalıştır, güvenli
kod sorunlarını düzelt ve Android build için manuel kontrol listesi ver.
Build çıktısını Git'e ekleme ve proje kapsamını genişletme.
```

---

## 6. Manuel Test Kontrol Listesi

### Başlangıç

- [ ] Yeni oyun tam olarak iki dolu hücreyle başlıyor.
- [ ] Başlangıç skoru `0`.
- [ ] İki sayı farklı hücrelerde.

### Hareket

- [ ] Dört yön doğru çalışıyor.
- [ ] Geçersiz hamle yeni sayı üretmiyor.
- [ ] Geçerli hamle tam olarak bir yeni sayı üretiyor.
- [ ] `2 2 2 2` sonucu `4 4` oluyor.
- [ ] Bir kutu aynı hamlede iki kez birleşmiyor.

### Durumlar

- [ ] Skor birleşen değer kadar artıyor.
- [ ] `2048` oluşunca kazanma gösteriliyor.
- [ ] Boş hücre veya birleşme varsa oyun bitmiyor.
- [ ] Hamle kalmayınca Game Over gösteriliyor.
- [ ] Restart temiz oyun başlatıyor.

### Input

- [ ] Ok tuşları çalışıyor.
- [ ] WASD çalışıyor.
- [ ] Dört swipe yönü çalışıyor.
- [ ] Kısa tap hamle oluşturmuyor.
- [ ] UI butonuna dokunmak hamle oluşturmuyor.

### Kayıt ve build

- [ ] High score uygulama yeniden açıldığında korunuyor.
- [ ] Portrait düzen farklı ekranlarda bozulmuyor.
- [ ] Android build alınabiliyor.
- [ ] Build çıktısı Git'e eklenmiyor.

---

## 7. Git Çalışma Düzeni

Her sprintten önce:

```bash
git status
```

Sprint tamamlandıktan ve Unity testleri geçtikten sonra:

```bash
git add Assets ProjectSettings Packages 2048_PROJECT_PLAN.md README.md .gitignore
git commit -m "SPRINT ICIN ONERILEN MESAJ"
```

Unity'nin ürettiği büyük klasörleri commit etme:

```text
Library/
Temp/
Logs/
obj/
Build/
Builds/
UserSettings/
```

---

## 8. Codex İçin Kısa Kullanım Şablonu

Her yeni Codex oturumunda yalnızca sprint numarasını değiştirerek şu komut kullanılabilir:

```text
2048_PROJECT_PLAN.md dosyasını tamamen oku.
Global Codex Kuralları ve Definition of Done bölümüne uy.
Yalnızca Sprint X.X'i uygula.
Önce mevcut projeyi incele, sonra gerekli değişiklikleri yap ve test et.
Gelecek sprintlerin özelliklerini ekleme.
Sprint sonunda plan dosyasındaki rapor formatını kullan.
```

Bu dosyadaki `[ ]` sprint durumu, yalnızca kabul kriterleri Unity Editor'da doğrulandıktan sonra `[x]` yapılmalıdır.
