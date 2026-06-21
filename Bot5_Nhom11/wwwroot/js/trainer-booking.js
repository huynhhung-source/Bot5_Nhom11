(() => {
    const modal = document.getElementById("trainerBookingModal");
    if (!modal) return;

    const state = {
        step: 1,
        packageName: "",
        packagePrice: 0,
        date: null,
        time: "",
        calendarMonth: new Date(new Date().getFullYear(), new Date().getMonth(), 1)
    };

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const steps = [...modal.querySelectorAll("[data-booking-step]")];
    const progressItems = [...modal.querySelectorAll("[data-progress-step]")];
    const nextButton = modal.querySelector("[data-booking-next]");
    const nextText = nextButton.querySelector("span");
    const backButton = modal.querySelector("[data-booking-back]");
    const calendarTitle = modal.querySelector("[data-calendar-title]");
    const calendarDays = modal.querySelector("[data-calendar-days]");
    const timeList = modal.querySelector("[data-time-list]");
    const studentName = modal.querySelector("[data-student-name]");
    const studentPhone = modal.querySelector("[data-student-phone]");
    const studentEmail = modal.querySelector("[data-student-email]");
    const studentNote = modal.querySelector("[data-student-note]");
    const formError = modal.querySelector("[data-form-error]");

    const timeSlots = ["06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00"];
    const bookedSlots = new Set(["08:00", "17:00", "19:00"]);

    const formatDate = (date) => new Intl.DateTimeFormat("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
    }).format(date);

    const formatPrice = (price) => `${new Intl.NumberFormat("vi-VN").format(price)}đ`;

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
        state.packageName = "";
        state.packagePrice = 0;
        state.date = null;
        state.time = "";
        state.calendarMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        studentName.value = "";
        studentPhone.value = "";
        studentEmail.value = "";
        studentNote.value = "";
        formError.classList.remove("show");
        modal.querySelectorAll(".booking-package.selected").forEach(item => item.classList.remove("selected"));
        renderCalendar();
        renderStep();
    }

    function renderStep() {
        steps.forEach(step => {
            step.classList.toggle("active", Number(step.dataset.bookingStep) === state.step);
        });

        progressItems.forEach(item => {
            const itemStep = Number(item.dataset.progressStep);
            item.classList.toggle("active", itemStep === state.step);
            item.classList.toggle("complete", itemStep < state.step);
            item.querySelector("span").textContent = itemStep < state.step ? "✓" : String(itemStep);
        });

        backButton.hidden = state.step === 1;
        nextText.textContent = state.step === 4 ? "Xác nhận đặt lịch" : "Tiếp tục";
        updateNextState();

        if (state.step === 2) {
            modal.querySelector("[data-date-summary]").textContent =
                `${formatDate(state.date)} — ${modal.dataset.trainerName}`;
            renderTimes();
        }

        if (state.step === 3) {
            modal.querySelector("[data-session-summary]").textContent =
                `${formatDate(state.date)} lúc ${state.time} — ${state.packageName}`;
        }

        if (state.step === 4) {
            fillConfirmation();
        }

        modal.querySelector(".booking-modal-body").scrollTop = 0;
    }

    function updateNextState() {
        if (state.step === 1) {
            nextButton.disabled = !(state.packageName && state.date);
        } else if (state.step === 2) {
            nextButton.disabled = !state.time;
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
        const currentMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        previousButton.disabled = state.calendarMonth <= currentMonth;

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
            button.disabled = date < today;

            if (date.getTime() === today.getTime()) button.classList.add("today");
            if (state.date && date.getTime() === state.date.getTime()) button.classList.add("selected");

            button.addEventListener("click", () => {
                state.date = date;
                state.time = "";
                renderCalendar();
                updateNextState();
            });
            calendarDays.appendChild(button);
        }
    }

    function renderTimes() {
        timeList.innerHTML = "";
        timeSlots.forEach(time => {
            const button = document.createElement("button");
            button.type = "button";
            button.className = "booking-time";
            button.innerHTML = bookedSlots.has(time) ? `${time}<small>Đã đặt</small>` : time;
            button.disabled = bookedSlots.has(time);
            button.classList.toggle("selected", state.time === time);
            button.addEventListener("click", () => {
                state.time = time;
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
        modal.querySelector("[data-confirm-date]").textContent = formatDate(state.date);
        modal.querySelector("[data-confirm-time]").textContent = state.time;
        modal.querySelector("[data-confirm-package]").textContent = state.packageName;
        modal.querySelector("[data-confirm-price]").textContent = formatPrice(state.packagePrice);
        modal.querySelector("[data-confirm-name]").textContent = studentName.value.trim();
        modal.querySelector("[data-confirm-phone]").textContent = studentPhone.value.trim();
    }

    function completeBooking() {
        closeModal();
        if (window.Swal) {
            Swal.fire({
                icon: "success",
                title: "Đặt lịch thành công",
                html: `Lịch tập với <strong>${modal.dataset.trainerName}</strong><br>${formatDate(state.date)} lúc ${state.time}`,
                confirmButtonColor: "#f36100"
            });
        } else {
            alert("Đặt lịch thành công. Huấn luyện viên sẽ sớm liên hệ với bạn.");
        }
    }

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
        state.calendarMonth = new Date(
            state.calendarMonth.getFullYear(),
            state.calendarMonth.getMonth() - 1,
            1
        );
        renderCalendar();
    });

    modal.querySelector("[data-calendar-next]").addEventListener("click", () => {
        state.calendarMonth = new Date(
            state.calendarMonth.getFullYear(),
            state.calendarMonth.getMonth() + 1,
            1
        );
        renderCalendar();
    });

    nextButton.addEventListener("click", () => {
        if (state.step === 3 && !validateStudent()) return;
        if (state.step === 4) {
            completeBooking();
            return;
        }

        state.step += 1;
        renderStep();
    });

    backButton.addEventListener("click", () => {
        if (state.step > 1) {
            state.step -= 1;
            renderStep();
        }
    });

    document.querySelectorAll(".js-open-trainer-booking").forEach(button => {
        button.addEventListener("click", openModal);
    });

    modal.querySelectorAll("[data-booking-close]").forEach(button => {
        button.addEventListener("click", closeModal);
    });

    document.addEventListener("keydown", event => {
        if (event.key === "Escape" && modal.classList.contains("open")) closeModal();
    });
})();
