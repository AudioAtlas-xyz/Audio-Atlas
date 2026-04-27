<template>
    <div :class="$style.stepper">
        <div :class="$style.step1">
            <div :class="$style.stepperItem">
                <div :class="$style.div">1</div>
            </div>
            <div :class="$style.titleDescription">
                <div :class="$style.address">Identity</div>
            </div>
        </div>
        <div :class="$style.separator" />
        <div :class="$style.step2">
            <div :class="$style.stepperItem2">
                <div :class="$style.div">2</div>
            </div>
            <div :class="$style.titleDescription">
                <div :class="$style.address">About</div>
            </div>
        </div>
        <div :class="$style.separator2" />
        <div :class="$style.step3">
            <div :class="$style.stepperItem2">
                <div :class="$style.div">3</div>
            </div>
            <div :class="$style.titleDescription">
                <div :class="$style.address">Connections</div>
            </div>
        </div>
    </div>
</template>
<style  module>.stepper {
    position: relative;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    isolation: isolate;
    text-align: left;
    font-size: 12px;
    color: #252c48;
    font-family: 'Space Grotesk';
}
    .step1 {
        width: 272px;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 6px;
        z-index: 5;
        color: #07080f;
    }
    .stepperItem {
        width: 24px;
        height: 24px;
        border-radius: 999px;
        background-color: #3de8c8;
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .div {
        position: relative;
        line-height: 16px;
        font-weight: 500;
    }
    .titleDescription {
        align-self: stretch;
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        text-align: center;
        color: #e4e8f5;
    }
    .address {
        align-self: stretch;
        position: relative;
        line-height: 16px;
        font-weight: 500;
    }
    .step2 {
        width: 272px;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 6px;
        z-index: 5;
    }
    .stepperItem2 {
        width: 24px;
        height: 24px;
        border-radius: 999px;
        background-color: #e4e8f5;
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .step3 {
        width: 272px;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 6px;
        z-index: 5;
    }
    .separator {
        height: 2px;
        width: 216px;
        position: absolute;
        margin: 0 !important;
        top: 11px;
        left: calc(50% - 240px);
        border-radius: 999px;
        background-color: #b0b8d4;
        z-index: 1;
    }
    .separator2 {
        height: 2px;
        width: 216px;
        position: absolute;
        margin: 0 !important;
        top: 11px;
        left: calc(50% + 24px);
        border-radius: 999px;
        background-color: #b0b8d4;
        z-index: 1;
    }

</style>
