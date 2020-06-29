namespace GaiaDB.Enums
{
    #region Enums

    /// <summary>
    /// Defines the eDescriptionKey.
    /// </summary>
    public enum eDescriptionKey
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// 심도
        /// </summary>
        Depth,
        /// <summary>
        /// 색조
        /// 색조 콘트롤 중 순차적으로 "첫째값" 또는 "첫째값~둘째값"
        /// </summary>
        Color,
        /// <summary>
        /// 두께
        /// </summary>
        Thick = 51,
        /// <summary>
        /// 여기부터 토사
        /// 통일분류.
        /// </summary>
        SoilColumn = 100,
        /// <summary>
        /// 함수상태
        /// 건조, 습윤, 젖음, 포화 중 순차적으로 "첫째값" 또는 "첫째값~둘째값"
        /// </summary>
        Wet,
        /// <summary>
        /// 경연상태
        /// 매우느슨, 느슨, 보통조밀, 조밀, 매우조밀 중 순차적으로 "첫째값" 또는 "첫째값~둘째값", 
        /// 매우연약, 연약, 보통견고, 견고, 매우견고, 고결 중 순차적으로 "첫째값" 또는 "첫째값~둘째값"
        /// </summary>
        Density,
        /// <summary>
        /// 여기부터 암반
        /// 대표암종
        /// 편마암, 편암, 각섬암, 석회암, 사암, 휘록암, 역암, 화강암, 섬록암, 감람암, 사문암, 유문암, 셰일, 안산암, 현무암
        /// </summary>
        RockType = 200,
        /// <summary>
        /// 풍화상태
        /// 신선, 약간풍화, 보통풍화, 심한풍화, 완전풍화, 잔적토 중 순차적으로 "첫째값" 또는 "첫째값~둘째값"
        /// </summary>
        Weathered,
        /// <summary>
        /// 강도상태
        /// 매우강함, 강함, 보통강함, 약함, 매우약함 중 순차적으로 "첫째값" 또는 "첫째값~둘째값" 
        /// </summary>
        Stiffness,
        /// <summary>
        /// 불연속면간격
        /// 200cm 이상, 60~200cm, 20~60cm, 6~20cm, 6cm 이하 중 순차적으로 "첫째값" 또는 "첫째값~둘째값"
        /// </summary>
        Gap,
        /// <summary>
        /// 절리
        /// 1절리군부터 3절리군까지 순차적으로 6개 값과 '/'를 구분자로 이루어진 리스트에 구분자 ','로 부분절리를 true/false로 갖음
        /// ex ) 1/2/3/4/5/6,true
        /// ex ) 10/20/30/40/50/60,false
        /// </summary>
        Joint,
        /// <summary>
        /// 절리면
        /// (계단형, 파동형, 평면형)/(거침, 완만, 경면),(계단형, 파동형, 평면형)/(거침, 완만, 경면)
        /// ex ) 계단형/거침,계단형/완만
        /// ex ) 파동형/거침,파동형/경면
        /// </summary>
        Rough,
        /// <summary>
        /// 암질 TCR (%)
        /// </summary>
        TCR = 300,
        /// <summary>
        /// 암질 RQD (%)
        /// </summary>
        RQD,
        /// <summary>
        /// 절리간격 최대값
        /// </summary>
        JointGapMin,
        /// <summary>
        /// 절리간격 최소값
        /// </summary>
        JointGapMax,
        /// <summary>
        /// 절리간격 평균값
        /// </summary>
        JointGapAvg,
        /// <summary>
        /// 비고
        /// </summary>
        Note = 1000,
    }

    #endregion
}
