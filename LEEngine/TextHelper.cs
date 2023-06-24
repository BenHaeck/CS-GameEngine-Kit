public class TextHelper {
	public static string GetRootPath => System.AppContext.BaseDirectory.ToString();

	// converts a string into a number, until it finds a character that is not a digit
	public static (int, int) GetNum(string s, int i) {
		int val = 0;
		while (i < s.Length) {
			if ('0' <= s[i] && s[i] <= '9') {
				val = val * 10 + (s[i] - '0');
			}
			else break;
			i++;
		}

		return (val, i);
	}

	
}