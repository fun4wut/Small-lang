
#[no_mangle]
pub extern fn print_int(x: i64) {
    println!("{}", x);
}


#[cfg(test)]
mod tests {
    #[test]
    fn it_works() {
        assert_eq!(2 + 2, 4);
    }
}
