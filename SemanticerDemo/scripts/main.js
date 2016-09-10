$(document)
    .ready(function() {
        $(function() {
            $('a[href*="#"]:not([href="#"])')
                .click(function() {
                    if (location.pathname.replace(/^\//, "") === this.pathname.replace(/^\//, "") &&
                        location.hostname === this.hostname) {
                        var target = $(this.hash);
                        target = target.length ? target : $("[name=" + this.hash.slice(1) + "]");
                        if (target.length) {
                            $("html, body")
                                .animate({
                                        scrollTop: target.offset().top
                                    },
                                    1000);
                            return false;
                        }
                    }
                });
            var chartData;
            $.ajaxSetup({ async: false });
            $.get("GetStats",
                function(data) {
                    chartData = data;
                });

            var randomColorFactor = function() {
                return Math.round(Math.random() * 255);
            };
            var randomColor = function(opacity) {
                return "rgba(" +
                    randomColorFactor() +
                    "," +
                    randomColorFactor() +
                    "," +
                    randomColorFactor() +
                    "," +
                    (opacity || ".3") +
                    ")";
            };


            var chartDataPositive = [];
            for (i = 0; i < chartData[1].Items.length; i++) {
                chartDataPositive.push(chartData[1].Items[i].Count);
            }
            var chartDataNegative = [];
            for (i = 0; i < chartData[1].Items.length; i++) {
                chartDataNegative.push(chartData[3].Items[i].Count);
            }
            var chartDataNeutral = [];
            for (i = 0; i < chartData[1].Items.length; i++) {
                chartDataNeutral.push(chartData[2].Items[i].Count);
            }
            var chartDataNotCalc = [];
            for (i = 0; i < chartData[1].Items.length; i++) {
                chartDataNotCalc.push(chartData[0].Items[i].Count);
            }

            var labels = [
                "0:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00",
                "12:00",
                "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
            ];
            var config = {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: chartData[1].Mark,
                            data: chartDataPositive,
                            fill: false,
                            borderColor: "#006600",
                            backgroundColor: "#006600",
                            pointBorderColor: "rgba(0,0,0,0.8)",
                            pointBackgroundColor: "#009933",
                            pointBorderWidth: 0.5
                        }, {
                            label: chartData[2].Mark,
                            data: chartDataNeutral,
                            fill: false,
                            borderColor: "#0033CC",
                            backgroundColor: "#0033CC",
                            pointBorderColor: "rgba(0,0,0,0.8)",
                            pointBackgroundColor: "#0000FF",
                            pointBorderWidth: 0.5
                        }, {
                            label: chartData[3].Mark,
                            data: chartDataNegative,
                            fill: false,
                            borderColor: "#CC0000",
                            backgroundColor: "#CC0000",
                            pointBorderColor: "rgba(0,0,0,0.8)",
                            pointBackgroundColor: "#FF5050",
                            pointBorderWidth: 0.5
                        }, {
                            label: chartData[0].Mark,
                            data: chartDataNotCalc,
                            fill: false
                        }
                    ]
                },
                options: {
                    responsive: true,
                    title: {
                        display: false
                    },
                    scales: {
                        xAxes: [
                            {
                                display: true,
                                ticks: {
                                    userCallback: function(dataLabel, index) {
                                        return index % 2 === 0 ? dataLabel : "";
                                    }
                                }
                            }
                        ],
                        yAxes: [
                            {
                                display: true,
                                beginAtZero: false
                            }
                        ]
                    }
                }
            };
            window.onload = function() {
                var ctx = document.getElementById("canvas").getContext("2d");
                window.myLine = new Chart(ctx, config);
            };


        });
    })