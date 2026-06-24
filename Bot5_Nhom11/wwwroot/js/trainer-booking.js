(() => {
    const modal = document.getElementById("trainerBookingModal");
    if (!modal) return;

    const classes = JSON.parse(modal.querySelector("[data-trainer-classes]")?.textContent || "[]");
    const state = {
        step: 1,
        roomId: "",
        packageName: "",
        packagePrice: 0,
        date: null,
        classId: null,
        calendarMonth: new Date(new Date().getFullYear(), new Date().getMonth(), 1)
    };

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const steps = [...modal.querySelectorAll("[data-booking-step]")];
    const progressItems = [...modal.querySelectorAll("[data-progress-step]")];
    const nextButton = modal.querySelector("[data-booking-next]");
    const nextText = nextButton.querySelector("span");
    const backButton = modal.querySelector("[data-booking-back]");
    const roomSelect = modal.querySelector("[data-booking-room]");
    const calendarTitle = modal.querySelector("[data-calendar-title]");
    const calendarDays = modal.querySelector("[data-calendar-days]");
    const timeList = modal.querySelector("[data-time-list]");
    const studentName = modal.querySelector("[data-student-name]");
    const studentPhone = modal.querySelector("[data-student-phone]");
    const studentEmail = modal.querySelector("[data-student-email]");
    const studentNote = modal.querySelector("[data-student-note]");
    const formError = modal.querySelector("[data-form-error]");

    const selectedClass = () => classes.find(item => item.classId === state.classId);
    const roomClasses = () => classes.filter(item => String(item.roomId) === String(state.roomId));
    const dateClasses = () => roomClasses().filter(item => item.date === toDateKey(state.date));

    const toDateKey = date => {
        if (!date) return "";
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        return `${year}-${month}-${day}`;
    };

    const formatDate = date => new Intl.DateTimeFormat("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
    }).format(date);

    const formatPrice = price => `${new Intl.NumberFormat("vi-VN").format(price)}đ`;

    function openModal() {
        modal.classList.add("open");
        modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("booking-modal-open");
        resetBooking();
    }

    function closeModal() {
        modal.classList.remove("open");
        modal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("booking-modal-open");
    }

    function resetBooking() {
        state.step = 1;
        state.roomId = "";
        state.packageName = "";
        state.packagePrice = 0;
        state.date = null;
        state.classId = null;
        state.calendarMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        if (roomSelect) roomSelect.value = "";
        studentName.value = "";
        studentPhone.value = "";
        studentEmail.value = "";
        studentNote.value = "";
        formError?.classList.remove("show");
        modal.querySelectorAll(".booking-package.selected").forEach(item => item.classList.remove("selected"));
        renderCalendar();
        renderStep();
    }

    function renderStep() {
        steps.forEach(step => step.classList.toggle("active", Number(step.dataset.bookingStep) === state.step));
        progressItems.forEach(item => {
            const itemStep = Number(item.dataset.progressStep);
            item.classList.toggle("active", itemStep === state.step);
            item.classList.toggle("complete", itemStep < state.step);
            item.querySelector("span").textContent = itemStep < state.step ? "✓" : String(itemStep);
        });

        backButton.hidden = state.step === 1;
        nextText.textContent = state.step === 4 ? "Thanh toán" : "Tiếp tục";

        if (state.step === 2) {
            modal.querySelector("[data-date-summary]").textContent =
                `${formatDate(state.date)} — ${modal.dataset.trainerName}`;
            renderTimes();
        }

        if (state.step === 3) {
            const item = selectedClass();
            modal.querySelector("[data-session-summary]").textContent =
                `${formatDate(state.date)} lúc ${item.startTime} — ${item.roomName}`;
        }

        if (state.step === 4) fillConfirmation();
        updateNextState();
        modal.querySelector(".booking-modal-body").scrollTop = 0;
    }

    function updateNextState() {
        if (state.step === 1) {
            nextButton.disabled = !(state.roomId && state.packageName && state.date);
        } else if (state.step === 2) {
            nextButton.disabled = !state.classId;
        } else {
            nextButton.disabled = false;
        }
    }

    function renderCalendar() {
        const year = state.calendarMonth.getFullYear();
        const month = state.calendarMonth.getMonth();
        calendarTitle.textContent = `Tháng ${month + 1} ${year}`;
        calendarDays.innerHTML = "";

        const previousButton = modal.querySelector("[data-calendar-prev]");
        previousButton.disabled = state.calendarMonth <= new Date(today.getFullYear(), today.getMonth(), 1);

        const availableDates = new Set(roomClasses().map(item => item.date));
        const firstWeekday = new Date(year, month, 1).getDay();
        const daysInMonth = new Date(year, month + 1, 0).getDate();

        for (let index = 0; index < firstWeekday; index += 1) {
            const empty = document.createElement("span");
            empty.className = "booking-day empty";
            calendarDays.appendChild(empty);
        }

        for (let day = 1; day <= daysInMonth; day += 1) {
            const date = new Date(year, month, day);
            const button = document.createElement("button");
            button.type = "button";
            button.className = "booking-day";
            button.textContent = String(day);
            button.disabled = date < today || !availableDates.has(toDateKey(date));
            if (state.date && toDateKey(date) === toDateKey(state.date)) button.classList.add("selected");
            button.addEventListener("click", () => {
                state.date = date;
                state.classId = null;
                renderCalendar();
                updateNextState();
            });
            calendarDays.appendChild(button);
        }
    }

    function renderTimes() {
        timeList.innerHTML = "";
        dateClasses().forEach(item => {
            const button = document.createElement("button");
            button.type = "button";
            button.className = "booking-time";
            button.innerHTML = `${item.startTime} - ${item.endTime}<small>${item.className} · Còn ${item.availableSlots} chỗ</small>`;
            button.classList.toggle("selected", state.classId === item.classId);
            button.addEventListener("click", () => {
                state.classId = item.classId;
                renderTimes();
                updateNextState();
            });
            timeList.appendChild(button);
        });
    }

    function validateStudent() {
        const name = studentName.value.trim();
        const phone = studentPhone.value.trim();
        const phonePattern = /^[0-9+\s.-]{9,15}$/;
        let message = "";
        if (!name) message = "Vui lòng nhập họ và tên.";
        else if (!phone) message = "Vui lòng nhập số điện thoại.";
        else if (!phonePattern.test(phone)) message = "Số điện thoại chưa đúng định dạng.";
        else if (studentEmail.value.trim() && !studentEmail.checkValidity()) message = "Email chưa đúng định dạng.";
        formError.textContent = message;
        formError.classList.toggle("show", Boolean(message));
        return !message;
    }

    function fillConfirmation() {
        const item = selectedClass();
        modal.querySelector("[data-confirm-date]").textContent = formatDate(state.date);
        modal.querySelector("[data-confirm-time]").textContent = `${item.startTime} - ${item.endTime}`;
        modal.querySelector("[data-confirm-room]").textContent = item.roomName;
        modal.querySelector("[data-confirm-class]").textContent = item.className;
        modal.querySelector("[data-confirm-package]").textContent = state.packageName;
        modal.querySelector("[data-confirm-price]").textContent = formatPrice(state.packagePrice);
        modal.querySelector("[data-confirm-name]").textContent = studentName.value.trim();
        modal.querySelector("[data-confirm-phone]").textContent = studentPhone.value.trim();
    }

    function goToPayment() {
        if (modal.dataset.isLoggedIn !== "true") {
            window.location.href = `${modal.dataset.loginUrl}?returnUrl=${encodeURIComponent(`/Payment/ClassCheckout?classId=${state.classId}`)}`;
            return;
        }
        window.location.href = `/Payment/ClassCheckout?classId=${state.classId}`;
    }

    roomSelect?.addEventListener("change", () => {
        state.roomId = roomSelect.value;
        state.date = null;
        state.classId = null;
        renderCalendar();
        updateNextState();
    });

    modal.querySelectorAll(".booking-package").forEach(button => {
        button.addEventListener("click", () => {
            modal.querySelectorAll(".booking-package").forEach(item => item.classList.remove("selected"));
            button.classList.add("selected");
            state.packageName = button.dataset.packageName;
            state.packagePrice = Number(button.dataset.packagePrice);
            updateNextState();
        });
    });

    modal.querySelector("[data-calendar-prev]").addEventListener("click", () => {
        state.calendarMonth = new Date(state.calendarMonth.getFullYear(), state.calendarMonth.getMonth() - 1, 1);
        renderCalendar();
    });
    modal.querySelector("[data-calendar-next]").addEventListener("click", () => {
        state.calendarMonth = new Date(state.calendarMonth.getFullYear(), state.calendarMonth.getMonth() + 1, 1);
        renderCalendar();
    });

    nextButton.addEventListener("click", () => {
        if (state.step === 3 && !validateStudent()) return;
        if (state.step === 4) return goToPayment();
        state.step += 1;
        renderStep();
    });

    backButton.addEventListener("click", () => {
        if (state.step > 1) {
            state.step -= 1;
            renderStep();
        }
    });

    document.querySelectorAll(".js-open-trainer-booking").forEach(button => button.addEventListener("click", openModal));
    modal.querySelectorAll("[data-booking-close]").forEach(button => button.addEventListener("click", closeModal));
    document.addEventListener("keydown", event => {
        if (event.key === "Escape" && modal.classList.contains("open")) closeModal();
    });
})();
