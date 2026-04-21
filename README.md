<h2>🎯 Projenin Amacı (Veli - Öğretmen İletişim Platformu)</h2>

<p>
Bu platform; <b>okul yönetimi (Admin), öğretmenler ve veliler</b> arasındaki iletişimi tek bir dijital çatı altında toplayarak
daha <b>şeffaf, hızlı ve organize</b> bir süreç oluşturmayı hedefler.
</p>

<p><b>Temel özellikler:</b></p>

<ul>
  <li>
    <b>📅 Akıllı Randevu Sistemi:</b>
    Velilerin, sadece kendi çocuklarının dersine giren öğretmenlerin müsaitlik durumlarını (bireysel veya grup dersleri) görüp sistem üzerinden hızlıca randevu talep edebilmesi.
  </li>

  <li>
    <b>🔔 Anlık Bildirimler (Real-Time):</b>
    Öğretmen bir randevuyu onayladığında veya reddettiğinde, velinin ekranına sayfa yenilemeye gerek kalmadan anında bildirim düşmesi.
  </li>

  <li>
    <b>👨‍🎓 Öğrenci ve Atama Yönetimi:</b>
    Öğrencilerin hangi öğretmenlerden ders aldığının sistem üzerinde eşleştirilmesi ve velilerin çocuklarının eğitim durumunu takip edebilmesi.
  </li>

  <li>
    <b>📢 Duyuru ve Şikayet Yönetimi:</b>
    Okul genelindeki duyuruların anlık olarak yayınlanması ve velilerden gelen talep/şikayetlerin kayıt altına alınarak yönetilmesi.
  </li>

  <li>
    <b>📝 Görüşme Notları:</b>
    Öğretmenlerin veli görüşmeleri sonrasında aldıkları notları dijital ortamda güvenli bir şekilde saklayabilmesi.
  </li>
</ul>

<hr/>

<h2>💻 Kullanılan Teknolojiler ve Mimari</h2>

<p>
Proje, güncel yazılım standartlarına uygun olarak <b>Clean Architecture</b> ve <b>CQRS</b> prensipleriyle full-stack bir yapı olarak geliştirilmiştir.
</p>

<hr/>

<h3>⚙️ Backend (Sunucu Tarafı)</h3>

<ul>
  <li><b>.NET 9 (C#):</b> Yüksek performanslı ve güvenilir Web API altyapısı</li>

  <li><b>Clean Architecture & CQRS (MediatR):</b>
    İş mantığının katmanlara ayrılması, Command (yazma) ve Query (okuma) işlemlerinin ayrıştırılması ile ölçeklenebilir yapı
  </li>

  <li><b>Entity Framework Core:</b> ORM yapısı ile veritabanı yönetimi</li>

  <li><b>ASP.NET Core Identity & JWT:</b> Kullanıcı kimlik doğrulama, rol yönetimi (Admin, Teacher, Parent) ve güvenli token tabanlı yapı</li>

  <li><b>SignalR:</b> Gerçek zamanlı bildirim ve iletişim altyapısı</li>

  <li><b>Hangfire:</b> Arka plan görevleri ve zamanlanmış işlemler (bildirim, hatırlatma vb.)</li>
</ul>

<hr/>

<h3>🎨 Frontend (Kullanıcı Arayüzü)</h3>

<ul>
  <li><b>Angular 21:</b> Modern, hızlı ve SPA (Single Page Application) mimarisi</li>

  <li><b>Angular Signals & Standalone Components:</b>
    Daha reaktif ve performanslı Angular mimarisi
  </li>

  <li><b>Zanex UI (Bootstrap 5):</b>
    Mobil uyumlu, modern ve kurumsal arayüz tasarımı
  </li>

  <li><b>SweetAlert2:</b>
    Kullanıcı bildirimleri, uyarılar ve onay pencereleri için modern popup sistemi
  </li>
</ul>
