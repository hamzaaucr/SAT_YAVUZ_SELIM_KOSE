⚔️ Legend of Selim: The Rogue Pursuit
Legend of Selim, Unity oyun motoru ve C# kullanılarak geliştirilmiş, derin hikaye anlatımı ve stratejik envanter yönetimine odaklanan bir 2D Sıra Tabanlı RPG (Role-Playing Game) projesidir. Oyuncuların Elias rehberliğinde başladığı bu yolculuk, basit bir hayatta kalma mücadelesinden devasa bir suç imparatorluğunu çökertme serüvenine dönüşür.

🌟 Öne Çıkan Özellikler
Merkezi Veritabanı Sistemi: Tüm eşya, düşman ve senaryo verileri GameManager üzerinden tek bir noktadan yönetilir.

Dinamik Dükkan ve Ekonomi: Alış ve satış fiyatları her eşya için özel olarak belirlenmiş olup, veritabanındaki güncellemeler dükkan arayüzüne anında yansır.

Stratejik Eğitim (Tutorial) Kilidi: Oyuncunun temel mekanikleri (kılıç kuşanma ve yetenek açma) öğrenmeden ilerlemesini engelleyen akıllı kilit sistemi.

Gelişmiş Diyalog Sistemi: Karakter görselleri ve isimlendirmeleriyle desteklenen, sürükleyici hikaye anlatımı.

Kademeli Zorluk Eğrisi: 10 farklı düşman türüyle zayıftan güçlüye doğru ilerleyen ve 4000 HP'lik devasa bir boss savaşıyla sonlanan dengeli oyun akışı.

🛠️ Yöntem ve Teknik Detaylar
Proje, modern oyun programlama prensipleri baz alınarak aşağıdaki teknolojilerle inşa edilmiştir:

Oyun Motoru: Unity 2022+ (2D Core)

Dil: C# (Object Oriented Programming)

Veri Yönetimi: [System.Serializable] sınıflar aracılığıyla yapılandırılmış veri modelleri ve Singleton tasarım deseni.

Kullanıcı Arayüzü: TextMeshPro, Grid Layout Group ve Scroll View bileşenleri kullanılarak oluşturulmuş dinamik ve duyarlı menüler.

Kalıcı Veri: Oyuncu ilerlemesi, envanter ve altın verileri JSON serileştirme ve PlayerPrefs kullanılarak saklanmaktadır.

🕹️ Oynanış ve Akış
Rehberlik: Elias ile başlayan diyaloglar oyuncuyu envantere yönlendirir.

Hazırlık: Oyuncu "Demir Kılıç" kuşanmalı ve "Normal Saldırı" yeteneğini açmalıdır.

Gelişim: Her savaştan kazanılan altınlarla dükkandan daha güçlü zırh ve kalkanlar satın alınabilir.

Final: Kademeli olarak artan düşman gücü, mağaranın sonundaki Hantal Haydup Patronu ile zirveye ulaşır.

📂 Kurulum
Bu repoyu bilgisayarınıza indirin veya klonlayın.

Unity Hub üzerinden projeyi açın.

Scenes/MenuScene sahnesine giderek "Play" butonuna basın.

📜 Lisans
Bu proje eğitim ve geliştirme amaçlı oluşturulmuştur. Görsel varlıklar (Kenney Assets vb.) kendi lisans şartlarına tabidir.
