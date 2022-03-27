import ApexCharts from "apexcharts";
window.ApexCharts = ApexCharts;

export const chart = (node, options) => {
    const myChart = new ApexCharts(node, options);

    const promise = myChart.render();

    promise.then(() => {
        myChart.windowResizeHandler();
        setTimeout(() => {
            myChart.el.style.visibility = null;
        }, 200);
    });

    return {
        update(options) {
            myChart.updateOptions(options);
        },
        destroy() {
            myChart.destroy();
        },
    };
};
