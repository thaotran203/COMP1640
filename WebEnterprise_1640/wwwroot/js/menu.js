function showmenu() {
    const show = document.querySelector(".show-menu");
    const home = document.getElementById("home");
    const login = document.getElementById("login");
    login.classList.toggle("an");
    home.classList.toggle("an");
    show.classList.toggle("none");
}