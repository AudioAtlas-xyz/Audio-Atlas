<script lang="js"></script>

<template>
  <UContainer :class ="$style.formsubmittedcard">
    <div class="relative flex items-center justify-center mb-6">
      <div :class="$style.ellipse">
        <div :class="$style.checkSymbol">✓</div>
      </div>
    </div>

    <div :class="$style.thanksAfrobeatsIs">Thanks! Afrobeats is now in review.</div>
    <div :class="$style.yourSubmissionHas">Your submission has been received and will be reviewed by our curatorial team. You'll be notified when it's approved or if the team has questions.</div>

    <div :class="$style.buttonAlone">
      <UButton type="submit" style="background-color: #3DE8C8">
        Submit another genre
      </UButton>
    </div>

  </UContainer>
</template>

<style module>
.formsubmittedcard {
width: 20%;
height: 380px;
position: relative;
border-radius: 6px;
background-color: #0d0f1a;
border: 1px solid #1c2038;
box-sizing: border-box;
overflow: hidden;
text-align: center;
font-size: 22px;
color: #3de8c8;
font-family: 'Space Grotesk';
margin: 10rem auto 0 auto;
}

.ellipse {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background-color: rgba(61, 232, 200, 0.08);
  border: 1px solid #3de8c8;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 44px auto 24px auto;
}
.checkSymbol {
  font-size: 24px;
  font-family: 'Space Grotesk';
  color: #3de8c8;
  line-height: 1;
}
.thanksAfrobeatsIs {
position: absolute;
top: 124px;
left: 6%;
font-size: 18px;
font-weight: 500;
color: #e4e8f5;
display: inline-block;
width: 339px;
height: 20px;
}
.yourSubmissionHas {
position: absolute;
top: 156px;
left: 12.5%;
font-size: 13px;
line-height: 178%;
font-weight: 300;
color: #7a84a8;
display: inline-block;
width: 279px;
height: 20px;
}
.btnprimary {
position: absolute;
top: 289px;
left: calc(50% - 70.5px);
border-radius: 6px;
background-color: #3de8c8;
width: 142px;
display: flex;
align-items: center;
padding: 4px 8px;
box-sizing: border-box;
font-size: 12px;
color: #07080f;
}
.button {
position: relative;
line-height: 16px;
font-weight: 500;
}

.buttonAlone {
display: flex;
justify-content: center;
align-items: center;
margin-top: 10rem;
}
</style>
