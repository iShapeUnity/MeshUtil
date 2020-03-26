namespace iShape.MeshUtil {

    public struct Segment {
        public readonly int begin;
        public readonly int length;
        public int end => begin + length - 1;

        public Segment(int begin, int length) {
            this.begin = begin;
            this.length = length;
        }
    }

}