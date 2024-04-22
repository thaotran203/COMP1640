function showmenu() {
  const show = document.querySelector(".show-menu");
  const home = document.getElementById("home");
  const login = document.getElementById("login");
  login.classList.toggle("an");
  home.classList.toggle("an");
  show.classList.toggle("none");
}
const drop = document.querySelector(".icon-drop");
const ul = document.querySelector(".drop-down ul");
drop.addEventListener("click", () => {
  ul.classList.toggle("drop");
})


//var slideIndex = 1;

//const data = [1, 1, 1, 1, 1, 1, 1, 1, 1];

//const show1 = document.getElementById("show1");

//data.forEach((item, index) => {
//  show1.innerHTML +=
//    `
//      <a href="./guest_article.html">
//        <div class="slide fade" >
//            <img src="https://img.meta.com.vn/Data/image/2022/01/13/anh-dep-thien-nhien-3.jpg" class="img-magazine">
//            <div style="font-size: 12px; margin-top: 5px"> 5 min read </div>
//            <div class="title-magazine">Magazines for graphic designers: The best graphic design publications</div>
//            <div class="description-magazine">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse varius enim in eros.</div>
//            <div class="read-more" style="display: flex; justify-content: center; align-item: center;"> 
//                Read more
//                <i class="fa-solid fa-caret-right" style="margin-left: 10px"></i>
//            </div>
//        </div>
//        </a>
//      `;
//})

//const slides = document.getElementsByClassName("slide");

//function plusSlides(n) {
//  showSlides(slideIndex += n);
//  console.log(slideIndex);
//}
//function showSlides(n) {
//  // 1 0 vs 1 0 thi 
//  if (window.matchMedia("(max-width: 1024px)").matches) { // Mobile
//    if (n == slides.length) {
//      slideIndex = 1;
//    }
//    if (n < 1) {
//      slideIndex = slides.length - 2;
//    }
//    for (i = 0; i < slides.length; i++) {
//      slides[i].style.display = "none";
//    }
//    slides[slideIndex - 1].style.display = "block";
//    slides[slideIndex].style.display = "block";
//  } else { // Laptop và các thiết bị lớn hơn
//    if (n == slides.length - 1) {
//      slideIndex = 1;
//    }
//    if (n < 1) {
//      slideIndex = slides.length - 2;
//    }
//    for (i = 0; i < slides.length; i++) {
//      slides[i].style.display = "none";
//    }
//    slides[slideIndex - 1].style.display = "block";
//    slides[slideIndex].style.display = "block";
//    slides[slideIndex + 1].style.display = "block";
//  }

//}
//showSlides(slideIndex);

//// --- show 2 ----
//const data1 = [1, 1, 1, 1, 1, 1, 1, 1, 1];
//var slideIndex1 = 1;

//const show2 = document.querySelector(".slideshow-container-2");
//console.log(show2);

//data1.forEach((item, index) => {
//  show2.innerHTML +=
//    `
//      <a href="./guest_article.html">
//        <div class="slide1 fade">
//            <div class="img-magazine">
//            <img src="https://cdn.trangcongnghe.com.vn/uploads/posts/2023-08/1692861228_image.png">
//            </div>
//            <div style="font-size: 12px; margin-top: 5px"> 5 min read </div>
//            <div class="title-magazine">Magazines for graphic designers: The best graphic design publications</div>
//            <div class="description-magazine">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse varius enim in eros.</div>
//            <div class="read-more" style="display: flex; justify-content: center; align-item: center;"> 
//                Read more
//                <i class="fa-solid fa-caret-right" style="margin-left: 10px"></i>
//            </div>
//          </div>
//        </a>
//      `;
//})
//const slides1 = document.getElementsByClassName("slide1");

//function plusSlides1(n) {
//  showSlides1(slideIndex1 += n);
//  console.log(slideIndex1);
//}
//function showSlides1(n) {

//  // 1 0 vs 1 0 thi 
//  if (window.matchMedia("(max-width: 1024px)").matches) { // Mobile

//    if (n == slides1.length - 1) {
//      slideIndex1 = 1;
//    }
//    if (n < 1) {
//      slideIndex1 = slides1.length - 2;
//    }
//    for (i = 0; i < slides1.length; i++) {
//      slides1[i].style.display = "none";

//    }
//    console.log(3, slides1[slideIndex1 - 1]);
//    slides1[slideIndex1 - 1].style.display = "block ";
//    console.log(slideIndex1);
//    slides1[slideIndex1].style.display = "block";
//  } else { // Laptop và các thiết bị lớn hơn
//    if (n == slides1.length - 1) {
//      slideIndex1 = 1;
//    }
//    if (n < 1) {
//      slideIndex1 = slides1.length - 2;
//    }
//    for (i = 0; i < slides1.length; i++) {
//      slides1[i].style.display = "none";
//    }
//    slides1[slideIndex1 - 1].style.display = "block";
//    slides1[slideIndex1].style.display = "block";
//    slides1[slideIndex1 + 1].style.display = "block";
//  }

//}
//showSlides1(slideIndex1);

//// ====== show3 =======

//// --- show 2 ----
//const data3 = [1, 1, 1, 1, 1, 1, 1, 1, 1];
//var slideIndex3 = 1;

//const show3 = document.querySelector(".slideshow-container-3");
//console.log(show3);

//data3.forEach((item, index) => {
//  show3.innerHTML +=
//    `
//      <a href="./guest_article.html">
//        <div class="slide3 fade">
//            <div class="img-magazine">
//            <img src="https://media-cdn-v2.laodong.vn/Storage/NewsPortal/2020/8/21/829850/Bat-Cuoi-Truoc-Nhung-07.jpg">
//            </div>
//            <div style="font-size: 12px; margin-top: 5px"> 5 min read </div>
//            <div class="title-magazine">Magazines for graphic designers: The best graphic design publications</div>
//            <div class="description-magazine">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse varius enim in eros.</div>
//            <div class="read-more" style="display: flex; justify-content: center; align-item: center;"> 
//                Read more
//                <i class="fa-solid fa-caret-right" style="margin-left: 10px"></i>
//            </div>
//          </div>
//        </a>
//      `;
//})
//const slides3 = document.getElementsByClassName("slide3");

//function plusSlides3(n) {
//  showSlides3(slideIndex3 += n);
//}
//function showSlides3(n) {

//  // 1 0 vs 1 0 thi 
//  if (window.matchMedia("(max-width: 1024px)").matches) { // Mobile

//    if (n == slides3.length - 1) {
//      slideIndex3 = 1;
//    }
//    if (n < 1) {
//      slideIndex3 = slides3.length - 2;
//    }
//    for (i = 0; i < slides3.length; i++) {
//      slides3[i].style.display = "none";

//    }
//    slides3[slideIndex3 - 1].style.display = "block ";

//    slides3[slideIndex3].style.display = "block";
//  } else { // Laptop và các thiết bị lớn hơn
//    if (n == slides3.length - 1) {
//      slideIndex3 = 1;
//    }
//    if (n < 1) {
//      slideIndex3 = slides3.length - 2;
//    }
//    for (i = 0; i < slides3.length; i++) {
//      slides3[i].style.display = "none";
//    }
//    slides3[slideIndex3 - 1].style.display = "block";
//    slides3[slideIndex3].style.display = "block";
//    slides3[slideIndex3 + 1].style.display = "block";
//  }

//}
//showSlides3(slideIndex3);