 document.addEventListener('DOMContentLoaded', function () {
  'use strict';
 
  window.addEventListener('load', function () {
    window.scrollTo(0, 0);
  }); 
   const backToTopBtn = document.getElementById('backToTop');
  window.addEventListener('scroll', () => {
    backToTopBtn.classList.toggle('visible', window.scrollY > 300);
  });
  backToTopBtn.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });
   const navLinks = document.querySelectorAll('.navbar-collapse .nav-link');
    const navbarCollapse = document.querySelector('.navbar-collapse');

    navLinks.forEach(function (link) {
      link.addEventListener('click', function () {
        if (window.innerWidth < 992) { 
          const bsCollapse = bootstrap.Collapse.getInstance(navbarCollapse);
          if (bsCollapse) {
            bsCollapse.hide();
          }
        }
      });
    });
    
 function formatNumber(num) {
  return num.toLocaleString() + '+';
}

function animateCounters() {
  const counters = document.querySelectorAll('.counter');
  counters.forEach(counter => {
    const target = +counter.getAttribute('data-target');
    let count = 0;

    const updateCount = () => {
      const current = +counter.innerText.replace(/[+,]/g, '');
      const increment = target / 100;

      if (current < target) {
        const newCount = Math.ceil(current + increment);
        counter.innerText = formatNumber(newCount);
        setTimeout(updateCount, 20);
      } else {
        counter.innerText = formatNumber(target);
      }
    };

    // Reset counter before animating
    counter.innerText = '0+';
    updateCount();
  });
}


const section = document.querySelector('.achievement-section');
let observer = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      animateCounters();
    }
  });
}, {
  threshold: 0.5 
});

if (section) {
  observer.observe(section);
}


  AOS.init({
    offset: 120,
    duration: 1000,
    easing: 'ease-in-out',
  });
});
