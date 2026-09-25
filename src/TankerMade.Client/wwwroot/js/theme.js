window.tankerMadeTheme = {
    set: function (theme) {
        document.documentElement.setAttribute('data-theme', theme === 'light' ? 'light' : 'dark');
    }
};
