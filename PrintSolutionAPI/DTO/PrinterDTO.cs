namespace PrintSolutionAPI.DTO
{
    /// <summary>
    /// 프린터 DTO
    /// </summary>
    public class PrinterDTO
    {
        /// <summary>
        /// 프린터 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 프린터 상태
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// 트린터 사용가능 여부
        /// </summary>
        public bool UseYN { get; set; }
    }
}
