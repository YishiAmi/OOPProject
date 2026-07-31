(() => {
  "use strict";

  const root = document.documentElement;
  const search = document.querySelector("#doc-search");
  const searchStatus = document.querySelector("#search-status");
  const searchItems = [...document.querySelectorAll("[data-search-item]")];
  const themeToggle = document.querySelector(".theme-toggle");
  const themeIcon = document.querySelector(".theme-icon");
  const mobileToggle = document.querySelector(".mobile-nav-toggle");
  const sidebar = document.querySelector(".sidebar");
  const dialog = document.querySelector(".diagram-dialog");
  const dialogContent = document.querySelector(".dialog-content");
  const dialogViewport = document.querySelector(".dialog-viewport");
  const dialogTitle = document.querySelector(".dialog-title");
  const zoomLevel = document.querySelector(".zoom-level");
  const zoomOut = document.querySelector("[data-zoom-out]");
  const zoomIn = document.querySelector("[data-zoom-in]");
  const zoomFit = document.querySelector("[data-zoom-fit]");
  const zoomActual = document.querySelector("[data-zoom-actual]");
  const closeDialog = document.querySelector(".dialog-close");
  let activeDiagram = null;
  let activeDiagramWidth = 0;
  let activeZoom = 1;

  const setTheme = (theme) => {
    root.dataset.theme = theme;
    const dark = theme === "dark";
    themeIcon.textContent = dark ? "☀" : "☾";
    themeToggle.setAttribute("aria-label", dark ? "Switch to light theme" : "Switch to dark theme");
    try {
      localStorage.setItem("rpg-docs-theme", theme);
    } catch {
      // A file:// page can deny storage; the visual toggle still works.
    }
  };

  let savedTheme = null;
  try {
    savedTheme = localStorage.getItem("rpg-docs-theme");
  } catch {
    savedTheme = null;
  }
  setTheme(savedTheme || (matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light"));

  themeToggle.addEventListener("click", () => {
    setTheme(root.dataset.theme === "dark" ? "light" : "dark");
  });

  const closeMobileNav = () => {
    sidebar.classList.remove("open");
    mobileToggle.setAttribute("aria-expanded", "false");
    mobileToggle.setAttribute("aria-label", "Open documentation navigation");
  };

  mobileToggle.addEventListener("click", () => {
    const opening = !sidebar.classList.contains("open");
    sidebar.classList.toggle("open", opening);
    mobileToggle.setAttribute("aria-expanded", String(opening));
    mobileToggle.setAttribute("aria-label", opening ? "Close documentation navigation" : "Open documentation navigation");
  });

  document.querySelectorAll(".side-nav a").forEach((link) => {
    link.addEventListener("click", closeMobileNav);
  });

  const normalize = (value) => value.toLocaleLowerCase().replace(/\s+/g, " ").trim();

  const filterReference = () => {
    const query = normalize(search.value);
    let matches = 0;
    document.body.classList.toggle("searching", Boolean(query));

    searchItems.forEach((item) => {
      const content = normalize(`${item.dataset.search || ""} ${item.textContent}`);
      const visible = !query || content.includes(query);
      item.hidden = !visible;
      if (visible) matches += 1;
    });

    if (!query) {
      searchStatus.textContent = "All reference entries shown";
    } else {
      searchStatus.textContent = `${matches} matching ${matches === 1 ? "entry" : "entries"}`;
    }
  };

  search.addEventListener("input", filterReference);
  document.addEventListener("keydown", (event) => {
    const tag = document.activeElement?.tagName;
    const typing = tag === "INPUT" || tag === "TEXTAREA";

    if (event.key === "/" && !typing) {
      event.preventDefault();
      search.focus();
    }

    if (event.key === "Escape") {
      if (dialog.open) {
        dialog.close();
      } else if (search.value) {
        search.value = "";
        filterReference();
        search.blur();
      } else {
        closeMobileNav();
      }
    }
  });

  document.querySelectorAll("pre").forEach((pre) => {
    const button = document.createElement("button");
    button.type = "button";
    button.className = "copy-button";
    button.textContent = "Copy";
    button.setAttribute("aria-label", "Copy code example");

    button.addEventListener("click", async () => {
      const value = pre.querySelector("code")?.textContent || pre.textContent;
      try {
        await navigator.clipboard.writeText(value);
        button.textContent = "Copied";
      } catch {
        button.textContent = "Select text";
      }
      setTimeout(() => {
        button.textContent = "Copy";
      }, 1600);
    });

    pre.append(button);
  });

  document.querySelectorAll("[data-diagram]").forEach((figure) => {
    const button = figure.querySelector(".expand-diagram");
    button.addEventListener("click", () => {
      const source = figure.querySelector(".plantuml-diagram");
      const clone = source.cloneNode(true);
      activeDiagram = clone;
      activeDiagramWidth = source.naturalWidth || 1200;
      dialogTitle.textContent = figure.querySelector("figcaption").textContent
        .replace("Standard UML", "")
        .trim();
      dialogContent.replaceChildren(clone);
      dialog.showModal();

      requestAnimationFrame(() => {
        const availableWidth = Math.max(320, dialogViewport.clientWidth - 32);
        activeZoom = Math.min(1, availableWidth / activeDiagramWidth);
        applyDiagramZoom(activeZoom);
        dialogViewport.scrollTo({ top: 0, left: 0 });
        closeDialog.focus();
      });
    });
  });

  const applyDiagramZoom = (zoom) => {
    if (!activeDiagram) return;
    activeZoom = Math.min(2, Math.max(0.2, zoom));
    activeDiagram.style.width = `${Math.round(activeDiagramWidth * activeZoom)}px`;
    zoomLevel.value = `${Math.round(activeZoom * 100)}%`;
  };

  zoomOut.addEventListener("click", () => applyDiagramZoom(activeZoom - 0.15));
  zoomIn.addEventListener("click", () => applyDiagramZoom(activeZoom + 0.15));
  zoomFit.addEventListener("click", () => {
    const availableWidth = Math.max(320, dialogViewport.clientWidth - 32);
    applyDiagramZoom(Math.min(1, availableWidth / activeDiagramWidth));
    dialogViewport.scrollTo({ top: 0, left: 0 });
  });
  zoomActual.addEventListener("click", () => applyDiagramZoom(1));

  closeDialog.addEventListener("click", () => dialog.close());
  dialog.addEventListener("close", () => {
    dialogContent.replaceChildren();
    activeDiagram = null;
  });
  dialog.addEventListener("click", (event) => {
    const rect = dialog.getBoundingClientRect();
    const outside = event.clientX < rect.left || event.clientX > rect.right ||
      event.clientY < rect.top || event.clientY > rect.bottom;
    if (outside) dialog.close();
  });

  const sectionLinks = [...document.querySelectorAll(".side-nav a[href^='#'], .page-toc a[href^='#']")];
  const sections = [...document.querySelectorAll("main > section[id]")];
  const linkMap = new Map();

  sectionLinks.forEach((link) => {
    const id = link.getAttribute("href").slice(1);
    const group = linkMap.get(id) || [];
    group.push(link);
    linkMap.set(id, group);
  });

  const observer = new IntersectionObserver((entries) => {
    const visible = entries
      .filter((entry) => entry.isIntersecting)
      .sort((a, b) => b.intersectionRatio - a.intersectionRatio)[0];
    if (!visible) return;

    sectionLinks.forEach((link) => link.classList.remove("active"));
    (linkMap.get(visible.target.id) || []).forEach((link) => link.classList.add("active"));
  }, { rootMargin: "-18% 0px -68% 0px", threshold: [0, 0.1, 0.35] });

  sections.forEach((section) => observer.observe(section));
  document.querySelector("#current-year").textContent = String(new Date().getFullYear());
})();
